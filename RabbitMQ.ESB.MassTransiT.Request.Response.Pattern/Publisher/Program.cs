
using MassTransit;
using Shared.RequestResponseMessage;

Console.WriteLine("Publisher");
string queueName = "request-response-queue";
var busControl=Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    cfg.Host("localhost", "/", h =>
    {
        h.Username("guest");
        h.Password("guest");
    });
});
await busControl.StartAsync();

var requestClient = busControl.CreateRequestClient<RequestMessage>(new Uri($"queue:{queueName}"));

int i = 1;
while (true)
{
    await Task.Delay(200);

    var response = await requestClient.GetResponse<ResponseMessage>(new RequestMessage { MessageNo = i, Text = $"Hello {i}" });
    Console.WriteLine($"Received response: {response.Message.Text}");
    i++;
}
Console.Read();