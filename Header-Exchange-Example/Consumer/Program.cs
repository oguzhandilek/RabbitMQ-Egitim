using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

ConnectionFactory factory = new()
{
    HostName = "localhost",
    Port = 5672,
    UserName = "guest",
    Password = "guest"
};

IConnection connection = await factory.CreateConnectionAsync();
IChannel channel = await connection.CreateChannelAsync();

string? exchange = "headers-exchange-example";
await channel.ExchangeDeclareAsync(
    exchange: exchange,
    type: ExchangeType.Headers
    );
Console.Write("Lütfen header value'sunu giriniz: ");
string? value=Console.ReadLine();
var queue = await channel.QueueDeclareAsync();
string? queueName = queue.QueueName;

channel.QueueBindAsync(
    queue: queueName,
    exchange: exchange,
    routingKey: string.Empty,
    new Dictionary<string, object>
    {
        ["x-match"] = "any",
        ["no"] = value
    });

AsyncEventingBasicConsumer consumer = new(channel);
await channel.BasicConsumeAsync(
    queue:queueName,
    autoAck:true,
    consumer:consumer);

consumer.ReceivedAsync += async (sender, e) =>
{
    string? message = Encoding.UTF8.GetString(e.Body.Span);
    Console.WriteLine(message);
};
Console.Read();

