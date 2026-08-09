namespace UsersService.Core.ServiceContracts
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string htmlBody);
        Task SendEmailVerificationLinkAsync(string toEmail, string userName, string verificationLink);
        Task SendPasswordResetLinkAsync(string toEmail, string userName, string resetLink);
    }
}
