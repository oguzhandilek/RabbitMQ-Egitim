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
string exchange = "fanout-exchange-exmple";
await channel.ExchangeDeclareAsync(exchange: exchange,
   type: ExchangeType.Fanout);

Console.WriteLine("Kuyruk Adını Giriniz: ");

string queueName=Console.ReadLine();

//Fanout exchange, mesajları tüm bağlı kuyruklara gönderir. RabbitMQ 4.x sürümlerinde bu kombinasyon (transient + non-exclusive) artık varsayılan olarak yasaklandı. Durable ve non-exclusive olarak kuyruk oluşturulmalıdır. Bu nedenle, kuyruk oluştururken durable: true ve exclusive: false parametreleri kullanılmalıdır. 
await channel.QueueDeclareAsync(queue: queueName,
    durable: true,
    exclusive: false, autoDelete: false);

await channel.QueueBindAsync(queue: queueName,
    exchange: exchange,
    routingKey: string.Empty);

AsyncEventingBasicConsumer consumer = new(channel);

await channel.BasicConsumeAsync(queue: queueName,
    autoAck: true,
    consumer: consumer);

consumer.ReceivedAsync += async (sender, e) =>
{
    string message = Encoding.UTF8.GetString(e.Body.Span);
    Console.WriteLine(message);
};

Console.Read();