//Procedur

using MassTransit;
using Shared.Messages;

string? queueName = "example-queue";
IBusControl busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    cfg.Host("localhost", "/", h => 
    {
        h.Username("guest");
        h.Password("guest");
    });
});

//Send'de mesaj göndeririken, Publish'de mesajı yayınlar. Publish, mesajı birden fazla alıcıya gönderebilir. Send ise belirli bir alıcıya mesaj gönderir.
ISendEndpoint sendEndpoint = await busControl.GetSendEndpoint(new Uri($"queue:{queueName}"));

Console.Write("Gönderilecek mesajı girin: ");
string? message = Console.ReadLine();

await sendEndpoint.Send<IMessage>(new ExampleMessage() { Text = message });
Console.Read();
