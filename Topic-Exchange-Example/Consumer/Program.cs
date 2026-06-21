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

IConnection connection= await factory.CreateConnectionAsync();
IChannel channel= await connection.CreateChannelAsync();

string? exchangeName = "topic-exchange-example";

await channel.ExchangeDeclareAsync(exchange: exchangeName,
    type: ExchangeType.Topic);
var queue = await channel.QueueDeclareAsync();
string? queueName=queue.QueueName;

Console.Write("mesaj alacğınız topic adını giriniz");
string? topic = Console.ReadLine();

await channel.QueueBindAsync(queue: queueName,
    exchange: exchangeName,
    routingKey: topic
    );

AsyncEventingBasicConsumer consumer = new(channel);

await channel.BasicConsumeAsync(
    queue: queueName,
    autoAck: true,
    consumer: consumer);

consumer.ReceivedAsync += async (sender, e) =>
{
   string? message = Encoding.UTF8.GetString(e.Body.Span);
   Console.WriteLine(message);
};

Console.Read();