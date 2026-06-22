////MassTransit 8.x → Açık kaynak (OSS), ücretsiz kullanılabiliyor.
//MassTransit 9.x → Ticari lisans gerektiren bir modele geçildi.
//Açık kaynak olarak geliştirilen son ana sürüm 8 tarafı kabul ediliyor.
//Birçok şirket şu anda hâlâ MassTransit 8 kullanıyor çünkü oldukça stabil ve olgun bir sürüm.

using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Publisher.Services;


var host= Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });
            });
        });
        services.AddHostedService<PublishMessageService>(provider=>
        {
            using IServiceScope scope = provider.CreateScope();
            IPublishEndpoint publishEndpoint = scope.ServiceProvider.GetService<IPublishEndpoint>()!;
            return new(publishEndpoint);
        });
    })
    .Build();

await host.RunAsync();