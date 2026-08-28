using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;

namespace OrdersService.Infrastructure.HttpClients
{
    public class BearerTokenPropagationHandler : DelegatingHandler
    {
        private const string AccessTokenCookieName = "access_token";

        private readonly IHttpContextAccessor _httpContextAccessor;

        public BearerTokenPropagationHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext is not null &&
                httpContext.Request.Cookies.TryGetValue(AccessTokenCookieName, out var accessToken) &&
                !string.IsNullOrEmpty(accessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
