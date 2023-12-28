using Mailjet.Client.TransactionalEmails;
using Mailjet.Client;
using Microsoft.AspNetCore.Identity.UI.Services;
using Mailjet.Client.Resources;
using Microsoft.Extensions.Configuration;

namespace SMS_Utility.EmailSending
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        private MailJetSettings _mailJetSettings;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return Execute(email, subject, htmlMessage);
        }

        public async Task Execute(string email, string subject, string htmlMessage)
        {
            _mailJetSettings = _configuration.GetSection("MailJet").Get<MailJetSettings>();

            MailjetClient client = new MailjetClient(_mailJetSettings.ApiKey, _mailJetSettings.SecretKey);

            MailjetRequest request = new MailjetRequest
            {
                Resource = Send.Resource
            };

            // construct your email with builder
            var emailToSent = new TransactionalEmailBuilder()
                   .WithFrom(new SendContact("shop.manager0711@gmail.com"))
                   .WithSubject(subject)
                   .WithHtmlPart(htmlMessage)
                   .WithTo(new SendContact(email))
                   .Build();

            // invoke API to send email
            var response = await client.SendTransactionalEmailAsync(emailToSent);
        }
    }
}
