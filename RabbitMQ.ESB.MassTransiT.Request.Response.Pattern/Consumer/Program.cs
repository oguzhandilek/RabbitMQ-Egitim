
using Consumer.Consumers;
using MassTransit;
Console.WriteLine("Consuemr");

string? queueName = "request-response-queue";
var busControl= Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    cfg.Host("localhost", "/", h =>
    {
        h.Username("guest");
        h.Password("guest");
    });
    cfg.ReceiveEndpoint(queueName, e =>
    {
        e.Consumer<RequestMessageConsumer>();
    });
});

await busControl.StartAsync();

Console.Read();