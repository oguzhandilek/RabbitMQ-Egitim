using System.Text;
using RabbitMQ.Client;

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
await channel.ExchangeDeclareAsync(
    exchange: exchangeName,
    type: ExchangeType.Topic);

for (int i = 0; i < 100; i++)
{
    Console.Write("Topic adını giriniz");
    string? topic=Console.ReadLine();
    byte[] message=Encoding.UTF8.GetBytes($"Merhaba, RabbitMQ {i}");
    await channel.BasicPublishAsync(exchangeName, routingKey: topic, body: message);
}
Console.Read();