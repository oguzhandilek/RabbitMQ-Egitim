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

// Declare exchange ile bir direct exchange oluşturuyoruz. Bu exchange, mesajları belirli bir routing key ile yönlendirecek.Type'i ExchangeType.Direct olarak belirliyoruz, bu sayede mesajlar sadece belirli bir routing key ile eşleşen kuyruklara gönderilecektir.
await channel.ExchangeDeclareAsync(exchange: "direct-exchange-example",
    type: ExchangeType.Direct);

while (true)
{
    Console.Write("Mesaj: ");
    string message=Console.ReadLine();
    byte[] byteMessage=Encoding.UTF8.GetBytes(message);

    await channel.BasicPublishAsync(exchange: "direct-exchange-example",
        routingKey:"direct-queue-example",
        body: byteMessage);
}
Console.Read();