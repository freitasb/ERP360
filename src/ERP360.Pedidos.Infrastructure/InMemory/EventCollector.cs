using ERP360.Pedidos.Application.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP360.Pedidos.Infrastructure.InMemory
{
    public sealed class EventCollector : IPublishEvent
    {
        public List<object> Published { get; } = new();
        public Task PublishAsync<T>(T message, CancellationToken ct = default)
        {
            Published.Add(message!);
            return Task.CompletedTask;
        }
    }
}
