using System.Collections.Concurrent;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using UsersService.Core.Options;
using UsersService.Core.ServiceContracts;

namespace UsersService.Core.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;
        private static readonly ConcurrentDictionary<string, string> TemplateCache = new();

        public EmailService(IOptions<EmailSettings> emailSettingsOptions, ILogger<EmailService> logger)
        {
            _emailSettings = emailSettingsOptions.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
                throw new ArgumentNullException(nameof(toEmail), "Recipient email address cannot be null or empty.");

            if (string.IsNullOrWhiteSpace(subject))
                throw new ArgumentNullException(nameof(subject), "Email subject cannot be null or empty.");

            if (string.IsNullOrWhiteSpace(htmlBody))
                throw new ArgumentNullException(nameof(htmlBody), "Email body cannot be null or empty.");

            try
            {
                var message = new MimeMessage();
                string senderEmail = string.IsNullOrWhiteSpace(_emailSettings.SenderEmail) ? _emailSettings.Username : _emailSettings.SenderEmail;
                message.From.Add(new MailboxAddress(_emailSettings.SenderName, senderEmail));
                message.To.Add(MailboxAddress.Parse(toEmail));
                message.Subject = subject;

                message.Body = new TextPart(TextFormat.Html)
                {
                    Text = htmlBody
                };

                using var client = new SmtpClient();

                SecureSocketOptions socketOptions = _emailSettings.EnableSsl
                    ? (_emailSettings.Port == 465 ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls)
                    : SecureSocketOptions.None;

                await client.ConnectAsync(_emailSettings.Host, _emailSettings.Port, socketOptions);

                if (!string.IsNullOrWhiteSpace(_emailSettings.Username) && !string.IsNullOrWhiteSpace(_emailSettings.Password))
                {
                    await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
                }

                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {ToEmail} with subject '{Subject}'.", toEmail, subject);
                throw new InvalidOperationException($"Failed to send email to {toEmail}");
            }
        }

        public async Task SendEmailVerificationLinkAsync(string toEmail, string userName, string verificationLink)
        {
            string displayName = string.IsNullOrWhiteSpace(userName) ? "Valued Customer" : userName;
            string templateContent = await ReadTemplateAsync("EmailVerificationTemplate.html");

            string htmlBody = templateContent
                .Replace("{{UserName}}", displayName)
                .Replace("{{VerificationLink}}", verificationLink)
                .Replace("{{Year}}", DateTime.UtcNow.Year.ToString());

            await SendEmailAsync(toEmail, "Verify Your Email Address", htmlBody);
        }

        public async Task SendPasswordResetLinkAsync(string toEmail, string userName, string resetLink)
        {
            string displayName = string.IsNullOrWhiteSpace(userName) ? "Valued Customer" : userName;
            string templateContent = await ReadTemplateAsync("PasswordResetTemplate.html");

            string htmlBody = templateContent
                .Replace("{{UserName}}", displayName)
                .Replace("{{ResetLink}}", resetLink)
                .Replace("{{Year}}", DateTime.UtcNow.Year.ToString());

            await SendEmailAsync(toEmail, "Reset Your Password", htmlBody);
        }

        private async Task<string> ReadTemplateAsync(string templateFileName)
        {
            if (TemplateCache.TryGetValue(templateFileName, out string? cachedTemplate))
            {
                return cachedTemplate;
            }

            string templatePath = Path.Combine(AppContext.BaseDirectory, "Templates", templateFileName);

            if (!File.Exists(templatePath))
            {
                _logger.LogError("Email template file not found at path: {TemplatePath}", templatePath);
                throw new FileNotFoundException($"Email template '{templateFileName}' was not found at '{templatePath}'.");
            }

            string templateContent = await File.ReadAllTextAsync(templatePath);
            TemplateCache.TryAdd(templateFileName, templateContent);

            return templateContent;
        }
    }
}
