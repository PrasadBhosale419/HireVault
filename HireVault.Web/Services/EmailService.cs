using SendGrid;
using SendGrid.Helpers.Mail;

namespace HireVault.Web.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsyc(string to, string subject, string body)
        {
            var apiKey = _configuration["SendGrid:ApiKey"];
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("prasad081200@gmail.com", "HireVault");
            var toEmail = new EmailAddress(to);

            var msg = MailHelper.CreateSingleEmail(from, toEmail, subject, plainTextContent: null, body);
            await client.SendEmailAsync(msg);
        }
    }
}
