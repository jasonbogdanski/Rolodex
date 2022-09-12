using NServiceBus;

namespace Rolodex.Messages
{
    public record SendEmail : ICommand
    {
        public string ToEmailAddress { get; init; } = null!;
        public string Subject { get; init; } = null!;
        public string Body { get; init; } = null!;
    }
}