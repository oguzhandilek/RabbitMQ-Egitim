using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Shared.Messages;

namespace Publisher.Services;

public class PublishMessageService : BackgroundService
{
    readonly IPublishEndpoint _publishEndpoint;

    public PublishMessageService(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        int i = 0;
        while (true)
        {
          ExampleMessage message = new()
          {
              Text = $"Message {i++}"
          };
           await _publishEndpoint.Publish(message, stoppingToken);
        }
    }
}
