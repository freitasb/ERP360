using ERP360.Pedidos.Application.Abstractions;
using ERP360.Pedidos.Application.Consumers;
using ERP360.Pedidos.Application.Pedidos.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP360.Pedidos.Infrastructure.EventBus
{
    public sealed class InMemoryEventBus : IPublishEvent
    {
        private readonly IEnumerable<IPedidoPagoConsumer> _consumers;

        public InMemoryEventBus(IEnumerable<IPedidoPagoConsumer> consumers)
        {
            _consumers = consumers;
        }

        public async Task PublishAsync<T>(T evento, CancellationToken ct)
        {
            if (evento is PedidoPago pedidoPago)
            {
                foreach (var consumer in _consumers)
                {
                    await consumer.HandleAsync(pedidoPago, ct);
                }
            }
        }
    }

}
