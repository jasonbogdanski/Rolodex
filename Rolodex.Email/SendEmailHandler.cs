using Azure.Communication.Email;
using Azure.Communication.Email.Models;
using Microsoft.Extensions.Options;
using NServiceBus;
using Rolodex.Messages;

namespace Rolodex.Email
{
    public class SendEmailHandler : IHandleMessages<SendEmail>
    {
        private readonly EmailCommunicationOptions _emailCommunicationOptions;

        public SendEmailHandler(IOptions<EmailCommunicationOptions> connectionStringOptions)
        {
            _emailCommunicationOptions = connectionStringOptions.Value;
        }

        public async Task Handle(SendEmail message, IMessageHandlerContext context)
        {
            var emailClient =
                new EmailClient(_emailCommunicationOptions.ConnectionString);

            var emailContent = new EmailContent(message.Subject)
            {
                Html = message.Body
            };

            var emailAddress = new List<EmailAddress> { new(message.ToEmailAddress) };
            var emailRecipients = new EmailRecipients(emailAddress);
            var emailMessage =
                new EmailMessage(_emailCommunicationOptions.ReplyToEmail
                    , emailContent
                    , emailRecipients);

            await emailClient.SendAsync(emailMessage);
        }
    }
}
