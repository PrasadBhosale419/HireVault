namespace HireVault.Web.Services
{
    public interface IEmailService
    {
        Task SendEmailAsyc(string to, string subject, string body);
    }
}
