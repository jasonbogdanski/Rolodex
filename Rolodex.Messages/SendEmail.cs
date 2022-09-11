using NServiceBus;

namespace Rolodex.Messages
{
    public record SendEmail : IEvent
    {
        public string ToEmailAddress { get; init; } = null!;
        public string Subject { get; init; } = null!;
        public string Body { get; init; } = null!;
    }
}