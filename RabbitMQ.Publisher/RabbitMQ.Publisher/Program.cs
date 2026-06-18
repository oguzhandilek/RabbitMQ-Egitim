
#region RabbitMQ.Client Kütüphanesi
//.NET teknolojileri ile RabbitMQ'yu kullanabilmek için RabbitMQ.Client kütüphanesini projenize yüklemeniz gerekmektedir.
#endregion
#region Publisher Uygulaması İşlem Sırası
//1. Bağlantı Oluşturma
//RabbitMQ SUnUCUSUna bağlantı oluşturunuz.
//2.Bağlantıyı Aktifleştirme ve Kanal Açma
//Bağlantıyı aktifleştiriniz ve ardından bu bağlantı üzerinden işlemleri yürütebilmek için bir kanal açınız.
//3.Queue Oluşturma
//Mesajların gönderileceği kuyruğu oluşturunuz.
//4.Queue'ya Mesaj Gönderme
//Kuyruğa mesaj gönderiniz.
#endregion
using RabbitMQ.Client;
using System.Text;

//Local de dockerize edilmiş RabbitMQ sunucusuna bağlanmak için aşağıdaki bağlantı ayarlarını kullanabilirsiniz.
ConnectionFactory factory = new()
{
    HostName = "localhost",
    Port = 5672,
    UserName = "guest",
    Password = "guest"
};

//ConnectionFactory factory = new();
//factory.Uri = new Uri("amqps://qtrldzyx:T-m3f2ulJj4YKH2N7PHvadw7nS-addVP@moose.rmq.cloudamqp.com/qtrldzyx"); // RabbitMQ sunucusunun URI'sini buraya giriniz.

//bağlantıyı aktifleştiriniz ve ardından bu bağlantı üzerinden işlemleri yürütebilmek için bir kanal açınız.
using IConnection connection = await factory.CreateConnectionAsync();

//Bağlantıyı aktifleştiriniz ve ardından bu bağlantı üzerinden işlemleri yürütebilmek için bir kanal açınız.
using IChannel channel = await connection.CreateChannelAsync();

//Parametre Açıklamaları:
//1. queue: Kuyruğun adı.
//2. durable: Kuyruğun kalıcı olup olmayacağını belirler. true ise kuyruk kalıcıdır ve RabbitMQ yeniden başlatıldığında kaybolmaz.
//3. exclusive: Kuyruğun yalnızca bu bağlantı tarafından kullanılabilir olup olmadığını belirler. true ise kuyruk yalnızca bu bağlantı tarafından kullanılabilir ve bağlantı kapatıldığında kuyruk silinir.
//4. autoDelete: Kuyruğun otomatik olarak silinip silinmeyeceğini belirler. true ise kuyruk, son tüketici bağlantısı kapatıldığında otomatik olarak silinir.
//5. arguments: Kuyruğun özelliklerini belirlemek için kullanılan ek argümanlar. Örneğin, TTL (Time-To-Live) veya maksimum uzunluk gibi özellikler burada belirtilebilir.
await channel.QueueDeclareAsync("example-queue", durable: true, exclusive: false, autoDelete: false, arguments: null);

//Queue'ya Mesaj Gönderme
//Queue'ya mesaj göndermek için mesajın byte[] formatında olması gerekmektedir. Bu nedenle mesajı UTF-8 formatında byte dizisine dönüştürmek için Encoding.UTF8.GetBytes() metodunu kullanabilirsiniz.

//byte[] message= Encoding.UTF8.GetBytes("Hello, RabbitMQ!"); // Gönderilecek mesajı byte[] formatına dönüştürür.
//await channel.BasicPublishAsync(exchange:"", routingKey:"example-queue", body:message); // Mesajı kuyruğa gönderir.

for (int i = 0; i < 1000; i++)
{
   await Task.Delay(1000);
    byte[] message = Encoding.UTF8.GetBytes($"Hello, RabbitMQ {i}");
    await channel.BasicPublishAsync(exchange: "", routingKey: "example-queue", body: message);
}

Console.ReadLine();