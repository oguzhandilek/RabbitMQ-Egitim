
using Consumer.Consumers;
using MassTransit;

string? queueName = "example-queue";

IBusControl bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    cfg.Host("localhost", "/", h =>
    {
        h.Username("guest");
        h.Password("guest");
    });
    cfg.ReceiveEndpoint(queueName, e =>
    {
       e.Consumer<ExampleMessageConsumer>();
    });
});

await bus.StartAsync();
Console.Read();
