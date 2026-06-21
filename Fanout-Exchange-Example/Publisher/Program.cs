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

//Fanout exchange, mesajları tüm bağlı kuyruklara gönderir. Routing key kullanılmaz.
await channel.ExchangeDeclareAsync(exchange: "fanout-exchange-exmple",
    type: ExchangeType.Fanout);

for (int i = 0; i < 500; i++)
{
    await Task.Delay(200);
    byte[] message = Encoding.UTF8.GetBytes($"Merhaba, RabbitMQ {i}");
    await channel.BasicPublishAsync(exchange: "fanout-exchange-exmple",
    routingKey: string.Empty,body:message);
}
Console.ReadLine();