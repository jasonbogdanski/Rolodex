using NServiceBus;

namespace Rolodex.IntegrationTests.Fakes
{
    public class MessageSessionFake : IMessageSession
    {
        public Action<object>? VerifySend { get; set; }

        public Task Send(object message, SendOptions options)
        {
            VerifySend?.Invoke(message);
            return Task.CompletedTask;
        }

        public Task Send<T>(Action<T> messageConstructor, SendOptions options)
        {
            return Task.CompletedTask;
        }

        public Task Publish(object message, PublishOptions options)
        {
            return Task.CompletedTask;
        }

        public Task Publish<T>(Action<T> messageConstructor, PublishOptions publishOptions)
        {
            return Task.CompletedTask;
        }

        public Task Subscribe(Type eventType, SubscribeOptions options)
        {
            return Task.CompletedTask;
        }

        public Task Unsubscribe(Type eventType, UnsubscribeOptions options)
        {
            return Task.CompletedTask;
        }
    }
}
