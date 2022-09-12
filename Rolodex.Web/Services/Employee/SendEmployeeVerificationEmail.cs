using MediatR;
using Microsoft.EntityFrameworkCore;
using NServiceBus;
using Rolodex.Messages;
using Rolodex.Web.DataStore;
using Rolodex.Web.Infrastructure;

namespace Rolodex.Web.Services.Employee
{
    public class SendEmployeeVerificationEmail
    {
        public record Command : IRequest<bool>
        {
            public int Id { get; init; }
        }

        public class CommandHandler : IRequestHandler<Command, bool>
        {
            private readonly RolodexContext _context;
            private readonly IMessageSession _messageSession;
            private readonly IGuidGenerator _guidGenerator;

            public CommandHandler(RolodexContext context, IMessageSession messageSession, IGuidGenerator guidGenerator)
            {
                _context = context;
                _messageSession = messageSession;
                _guidGenerator = guidGenerator;
            }

            public async Task<bool> Handle(Command message, CancellationToken token)
            {
                var employee = await _context.Employees
                    .Where(x => x.Id == message.Id)
                    .SingleAsync(cancellationToken: token);

                var verifyEmailCode = _guidGenerator.NewGuid();

                var sendEmail = new SendEmail
                {
                    ToEmailAddress = employee.Email,
                    Subject = "Verify your email",
                    Body =
                        $"Verify your email by following the link https://localhost:7150/VerifyEmail?emailCode={verifyEmailCode}"
                };

                await _messageSession.Send(sendEmail);

                return true;
            }
        }
    }
}
