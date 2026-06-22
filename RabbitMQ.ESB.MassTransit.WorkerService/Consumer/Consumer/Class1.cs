using MassTransit;
using Shared.Messages;

namespace Consumer.Consumer
{
    internal class ExampleMessageConsumer : IConsumer<IMessage>
    {
        public Task Consume(ConsumeContext<IMessage> context)
        {
            Console.WriteLine($"Received message: {context.Message.Text}");
            return Task.CompletedTask;
        }
    }
}
