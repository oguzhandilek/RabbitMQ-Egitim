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
IChannel channel= await connection.CreateChannelAsync();

#region P2P (Point-to-Point) Tasarımı

//string? queueName = "example-p2p-queue";

//await channel.QueueDeclareAsync(
//    queue: queueName,
//    durable: true,
//    exclusive: false,
//    autoDelete: false);

//byte[] message = Encoding.UTF8.GetBytes("Merhaba, P2P");

//await channel.BasicPublishAsync(exchange: string.Empty,
//    routingKey: queueName,
//    body: message);

#endregion

#region Pub/Sub (Publish/Subscribe) Tasarımı
//string? exchangeName = "example-pub-sub-exchange";
//await channel.ExchangeDeclareAsync(
//    exchange: exchangeName,
//    type: ExchangeType.Fanout);
//byte[] message = Encoding.UTF8.GetBytes("Merhaba Pub-Sub");
//await channel.BasicPublishAsync(
//    exchange: exchangeName,
//    routingKey: string.Empty,
//    body: message);
#endregion

#region Work Queue Tasarımı

//string? queueName = "example-work-queue";
//await channel.QueueDeclareAsync(
//    queue: queueName,
//    durable: true,
//    exclusive: false,
//    autoDelete: false);
//for (int i = 0; i < 100; i++)
//{
//    await Task.Delay(200);
//    byte[] message = Encoding.UTF8.GetBytes($"Merhaba, Work-Queue {i}");

//    await channel.BasicPublishAsync(
//        exchange: string.Empty,
//        routingKey: queueName,
//        body: message);

//}
#endregion

#region Request - Response Tasarımı

string? requestQueueName = "example-request-response-queue";

await channel.QueueDeclareAsync(
    queue: requestQueueName,
    durable: true,
    exclusive: false,
    autoDelete: false);

var queue = await channel.QueueDeclareAsync();

string? replyQueueName = queue.QueueName;

string? correlationId=Guid.NewGuid().ToString();

#region Request Mesajını Oluşturma ve Gönderme Davranışı

 BasicProperties properties = new BasicProperties();
properties.CorrelationId = correlationId;
properties.ReplyTo= replyQueueName;

for (int i = 0; i < 10; i++)
{
    await Task.Delay(200);
    byte[] message = Encoding.UTF8.GetBytes("Merhaba, Request-Response" + i);
    await channel.BasicPublishAsync(
        exchange: string.Empty,
        routingKey: requestQueueName,
        body: message,
        mandatory: true,
        basicProperties:properties);


}
#endregion

#region Response Kuyruğu Dineleme Davranışı

AsyncEventingBasicConsumer consumer=new(channel);


 await channel.BasicConsumeAsync(
     queue:replyQueueName,
     autoAck:true,
     consumer:consumer);

consumer.ReceivedAsync += async (sender, e) =>
{
    if (e.BasicProperties.CorrelationId == correlationId)
        Console.WriteLine(e.BasicProperties.CorrelationId);
        Console.WriteLine($"Response: {Encoding.UTF8.GetString(e.Body.Span)}");
};
#endregion


#endregion

Console.Read();