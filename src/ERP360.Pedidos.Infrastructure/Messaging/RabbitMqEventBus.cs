using ERP360.Pedidos.Application.Abstractions;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP360.Pedidos.Infrastructure.Messaging
{
    public sealed class RabbitMqEventBus : IPublishEvent
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public RabbitMqEventBus(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task PublishAsync<T>(T message, CancellationToken ct = default)
        {
            if (message is null)
                throw new ArgumentNullException(nameof(message));

            await _publishEndpoint.Publish(message!, ct);
        }
    }
}
