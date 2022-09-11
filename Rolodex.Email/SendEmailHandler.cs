using NServiceBus;
using Rolodex.Messages;

namespace Rolodex.Email
{
    public class SendEmailHandler : IHandleMessages<SendEmail>
    {
        public Task Handle(SendEmail message, IMessageHandlerContext context)
        {
            throw new NotImplementedException();
        }
    }
}
