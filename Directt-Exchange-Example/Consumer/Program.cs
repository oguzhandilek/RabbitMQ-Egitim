using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

ConnectionFactory factory = new()
{
    HostName = "localhost",
    Port = 5672,
    UserName = "guest",
    Password = "guest",

};
IConnection connection= await factory.CreateConnectionAsync();
IChannel channel=await connection.CreateChannelAsync();

//1. Adım: Exchange oluşturma. Publisher tarafında oluşturduğumuz exchange ile aynı isimde ve aynı tipte bir exchange oluşturuyoruz. Bu sayede mesajlar doğru exchange üzerinden yönlendirilecektir.
await channel.ExchangeDeclareAsync(exchange: "direct-exchange-example",
    type: ExchangeType.Direct);
//2.Adım: Queue oluşturma. Bu adımda, mesajları alacak olan kuyruk oluşturuluyor. Kuyruk ismi otomatik olarak oluşturulacak ve queueName değişkenine atanacaktır.
var queue = await channel.QueueDeclareAsync();
string queueName = queue.QueueName;

//3.Adım: Queue'yu exchange'e bağlama. Bu adımda, oluşturduğumuz kuyruk ile exchange arasında bir bağlantı kuruyoruz. Bu sayede, exchange üzerinden gelen mesajlar doğru kuyruklara yönlendirilecektir.
await channel.QueueBindAsync(queue: queueName,
    exchange: "direct-exchange-example",
    routingKey: "direct-queue-example");

AsyncEventingBasicConsumer consumer = new(channel: channel);
await channel.BasicConsumeAsync(queue:queueName,
    autoAck:true,
    consumer:consumer);

 consumer.ReceivedAsync += async (sender, e) =>
{
    string message = Encoding.UTF8.GetString(e.Body.Span);
    Console.WriteLine(message);
   
};
Console.Read();

