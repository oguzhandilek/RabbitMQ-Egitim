using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using Shared.RequestResponseMessage;

namespace Consumer.Consumers
{
    public class RequestMessageConsumer : IConsumer<RequestMessage>
    {
        public async Task Consume(ConsumeContext<RequestMessage> context)
        {
            Console.WriteLine(context.Message.Text);
            await context.RespondAsync<ResponseMessage>(new ResponseMessage { Text = $"Response to {context.Message.MessageNo}" });
        }
    }
}
