using System.Text;
using RabbitMQ.Client;

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


for (int i = 0; i < 100; i++)
{
    await Task.Delay( 100 );
    byte[] message = Encoding.UTF8.GetBytes($"Merhaba, RabbitMQ {i}");
    Console.Write("Lütfen header value'nuzu giriniz: ");
    string? value=Console.ReadLine();

BasicProperties basicProperties = new BasicProperties();
basicProperties.Headers=new Dictionary<string, object>
{
    
    ["no"]=value 
};

  await  channel.BasicPublishAsync(
        exchange: exchange,
        routingKey: string.Empty,
        body: message,
        mandatory: true,
        basicProperties: basicProperties);
}
Console.Read();