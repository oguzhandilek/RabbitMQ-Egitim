#region Consumer Uygulaması İşlem Sırası
//1.Bağlantı Oluşturma
//RabbitMQ sunucusuna bağlantı oluşturunuz.
//2.Bağlantıyı Aktifleştirme ve Kanal Açma
//Bağlantıyı aktifleştiriniz ve ardından bu bağlantı üzerinden işlemleri yürütebilmek için bir kanal açınız.
//3.Queue Oluşturma
//Mesajların okunacağı kuyruğu oluşturunuz.
//4.Queueldan Mesaj Okuma
//Kuyruktaki mesajları Okuyunuz.
#endregion

using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

//Local de dockerize edilmiş RabbitMQ sunucusuna bağlanmak için aşağıdaki bağlantı ayarlarını kullanabilirsiniz.
ConnectionFactory factory = new()
{
    HostName = "localhost",
    Port = 5672,
    UserName = "guest",
    Password = "guest"
};

//bağlantı oluşturma
//ConnectionFactory factory = new();
//factory.Uri = new("amqps://qtrldzyx:T-m3f2ulJj4YKH2N7PHvadw7nS-addVP@moose.rmq.cloudamqp.com/qtrldzyx");

//bağlantı aktifleştirme ve kanal açma

using IConnection connection= await factory.CreateConnectionAsync();
using IChannel channel= await connection.CreateChannelAsync();

//Queue oluşturma
await channel.QueueDeclareAsync("example-queue", durable: true, exclusive: false, autoDelete: false, arguments: null);
//Consumer'da da kuyruk publisher'daki ile birebir aynı yapılandırmada tanımlanmalıdır.

//Queue'dan mesaj okuma

AsyncEventingBasicConsumer consumer = new(channel);
await channel.BasicConsumeAsync(queue:"example-queue",autoAck:false,consumer:consumer);
await channel.BasicQosAsync(prefetchSize:0, prefetchCount:1, global:false); //Bu ayar, tüketicinin aynı anda kaç mesajı işleyebileceğini belirler.
                                                                      //prefetchSize:0, mesaj boyutu sınırlaması olmadığını belirtir. Bu, tüketicinin herhangi bir boyuttaki mesajı alabileceği anlamına gelir.
                                                                      //prefetchCount:1, tüketicinin aynı anda yalnızca bir mesajı işlemesine izin verir. Bu, mesajların sırayla işlenmesini sağlar ve her mesajın başarıyla işlendiğinden emin olunmasını sağlar.
                                                                      //global:false, bu ayarın yalnızca bu tüketici için geçerli olduğunu belirtir. Eğer true olsaydı, bu ayar tüm tüketiciler için geçerli olurdu.


//Mesajları okuma işlemi, mesaj geldiğinde tetiklenecek bir olay işleyicisi (event handler) aracılığıyla gerçekleştirilir. Bu işleyici, mesajın içeriğini alır ve istediğiniz şekilde işleyebilir. Örneğin, mesajı konsola yazdırabilirsiniz.
consumer.ReceivedAsync += async (sender, eventArgs) =>
{
    //Kuyruğa gelen mesajın işlendiği yerdir.
    //eventArgs.Body, mesajın içeriğini byte[] formatında içerir. Bu nedenle, mesajı okunabilir bir formata dönüştürmek için Encoding.UTF8.GetString() metodunu kullanabilirsiniz.
    // yada eventArgs.Body.Span kullanarak da mesajın içeriğini okuyabilirsiniz.
    string message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
    Console.WriteLine($"Received message: {message}");
    //Mesajın işlendiğini RabbitMQ'ya bildirmek için BasicAck metodunu kullanabilirsiniz. Bu, mesajın başarıyla işlendiğini ve kuyruğun bir sonraki mesajı göndermeye hazır olduğunu belirtir.
    await channel.BasicAckAsync(deliveryTag: eventArgs.DeliveryTag, multiple: false);

};
Console.ReadLine();