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

        public Task PublishAsync<T>(T message, CancellationToken ct = default)
            => _publishEndpoint.Publish(message, ct);
    }
}
