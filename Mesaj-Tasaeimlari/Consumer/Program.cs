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

IConnection connection=await factory.CreateConnectionAsync();
IChannel channel=await connection.CreateChannelAsync();

#region P2P (Point-to-Point) Tasarımı
//string? queueName = "example-p2p-queue";

//await channel.QueueDeclareAsync(
//   queue:queueName,
//   durable:true,
//   exclusive:false,
//   autoDelete:false);

//AsyncEventingBasicConsumer consumer = new(channel);
//await channel.BasicConsumeAsync(queue:queueName,
//    autoAck:false,
//    consumer:consumer);

//consumer.ReceivedAsync += async (sender, e) =>
//{
//    Console.WriteLine(Encoding.UTF8.GetString(e.Body.Span));
//};

#endregion

#region Pub-sub Tasarımı
//string? exchangeName = "example-pub-sub-exchange";

//await channel.ExchangeDeclareAsync(
//    exchange: exchangeName,
//    type: ExchangeType.Fanout);
//var queue = await channel.QueueDeclareAsync(
//  );
//string? queueName = queue.QueueName;

//await channel.QueueBindAsync(
//    queue: queueName,
//    exchange: exchangeName,
//    routingKey: string.Empty);
//await channel.BasicQosAsync(
//    prefetchSize:0,
//    prefetchCount:1,
//    global:false);
//AsyncEventingBasicConsumer consumer=new(channel);

//await channel.BasicConsumeAsync(
//    queue:queueName,
//    autoAck:true,
//    consumer:consumer);

//consumer.ReceivedAsync += async (sender, e) =>
//{
//    Console.WriteLine(Encoding.UTF8.GetString(e.Body.Span));
//};
#endregion

#region Work Queue Tasarımı

//string? queueName = "example-work-queue";
//await channel.QueueDeclareAsync(
//    queue: queueName,
//    durable: true,
//    exclusive: false,
//    autoDelete: false);

//AsyncEventingBasicConsumer consumer=new(channel);
//await channel.BasicConsumeAsync(
//    queue:queueName,
//    autoAck:true,
//    consumer:consumer);

//await channel.BasicQosAsync(
//    prefetchCount: 1,
//    prefetchSize: 0,
//    global: false);

//consumer.ReceivedAsync += async (sender, e) =>
//{
//    Console.WriteLine(Encoding.UTF8.GetString(e.Body.Span));
//};

#endregion

#region Request - Response Tasarımı

string? requestQueueName = "example-request-response-queue";

await channel.QueueDeclareAsync(
    queue: requestQueueName,
    durable: true,
    exclusive: false,
    autoDelete: false);

AsyncEventingBasicConsumer consumer = new(channel);

await channel.BasicConsumeAsync(
    queue: requestQueueName,
    autoAck: true,
    consumer: consumer);

consumer.ReceivedAsync += async(sender, e) =>
{
    string? message = Encoding.UTF8.GetString(e.Body.Span);

    Console.WriteLine($"message: {message} correlationId: {e.BasicProperties.CorrelationId}");
    
    byte[] responseMessage = Encoding.UTF8.GetBytes($"İşlem Tamamlandı. :{message}");
    BasicProperties properties = new BasicProperties();
    properties.CorrelationId=e.BasicProperties.CorrelationId;
    

    await channel.BasicPublishAsync(
        exchange: string.Empty,
        routingKey:e.BasicProperties.ReplyTo,
        mandatory:false,
        basicProperties:properties,
        body:responseMessage
        
     );

};
#endregion

Console.Read();