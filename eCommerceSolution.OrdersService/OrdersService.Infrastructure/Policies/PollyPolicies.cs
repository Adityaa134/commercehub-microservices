using System.Collections.Concurrent;
using System.Net;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Timeout;

namespace OrdersService.Infrastructure.Policies
{
    public class PollyPolicies
    {
        private readonly ILogger<PollyPolicies> _logger;
        private readonly ConcurrentDictionary<string, IAsyncPolicy<HttpResponseMessage>> _policyCache = new();

        public PollyPolicies(ILogger<PollyPolicies> logger)
        {
            _logger = logger;
        }

        private static bool IsTransientFailure(HttpResponseMessage r) =>
            (int)r.StatusCode >= 500 ||
            r.StatusCode == HttpStatusCode.RequestTimeout ||
            r.StatusCode == HttpStatusCode.TooManyRequests;

        public IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(string serviceName, int retryCount = 3)
        {
            var delay = Backoff.DecorrelatedJitterBackoffV2(
                medianFirstRetryDelay: TimeSpan.FromMilliseconds(200),
                retryCount: retryCount)
                .Select(d => d > TimeSpan.FromSeconds(1) ? TimeSpan.FromSeconds(1) : d);

            return Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                .Or<TimeoutRejectedException>()
                .OrResult(IsTransientFailure)
                .WaitAndRetryAsync(delay,
                    onRetry: (outcome, timespan, retryAttempt, context) =>
                    {
                        string reason = outcome.Exception?.Message 
                                     ?? outcome.Result?.StatusCode.ToString() 
                                     ?? "Unknown failure";

                        _logger.LogWarning(
                            "[{Service}] Retry {RetryAttempt} after {Delay}ms. Reason: {Reason}",
                            serviceName, retryAttempt, timespan.TotalMilliseconds, reason);
                    });
        }

        public IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(
            string serviceName,
            double failureThreshold = 0.5,
            int samplingDurationSeconds = 60,
            int minimumThroughput = 10,
            TimeSpan? durationOfBreak = null)
        {
            return Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                .Or<TimeoutRejectedException>()
                .OrResult(IsTransientFailure)
                .AdvancedCircuitBreakerAsync(
                    failureThreshold: failureThreshold,
                    samplingDuration: TimeSpan.FromSeconds(samplingDurationSeconds),
                    minimumThroughput: minimumThroughput,
                    durationOfBreak: durationOfBreak ?? TimeSpan.FromSeconds(30),
                    onBreak: (outcome, timespan) =>
                    {
                        string reason = outcome.Exception?.Message 
                                     ?? outcome.Result?.StatusCode.ToString() 
                                     ?? "Unknown failure";

                        _logger.LogCritical(
                            "[{Service}] CRITICAL: Circuit OPEN for {Duration}s. Calls will fail fast. Reason: {Reason}",
                            serviceName, timespan.TotalSeconds, reason);
                    },
                    onReset: () =>
                    {
                        _logger.LogInformation(
                            "[{Service}] Circuit CLOSED. Traffic restored.",
                            serviceName);
                    },
                    onHalfOpen: () =>
                    {
                        _logger.LogWarning(
                            "[{Service}] Circuit HALF-OPEN. Testing downstream health.",
                            serviceName);
                    });
        }

        public IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy(TimeSpan timeOut)
        {
            return Policy.TimeoutAsync<HttpResponseMessage>(timeOut, TimeoutStrategy.Optimistic);
        }

        public IAsyncPolicy<HttpResponseMessage> GetCombinedPolicy(
           string serviceName,
           int retryCount = 3,
           double failureThreshold = 0.5,
           int samplingDurationSeconds = 60,
           int minimumThroughput = 10,
           TimeSpan? durationOfBreak = null,
           TimeSpan? timeout = null)
        {
            return _policyCache.GetOrAdd(serviceName, _ =>
            {
                var retry = GetRetryPolicy(serviceName, retryCount);
                var breaker = GetCircuitBreakerPolicy(serviceName, failureThreshold, samplingDurationSeconds, minimumThroughput, durationOfBreak);
                var timeoutPolicy = GetTimeoutPolicy(timeout ?? TimeSpan.FromSeconds(2));

                return Policy.WrapAsync(retry, breaker, timeoutPolicy);
            });
        }
    }
}
