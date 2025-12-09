using ERP360.Pedidos.Domain.Primitives;
using ERP360.Pedidos.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP360.Pedidos.Domain.Events
{
    public sealed record PedidoCriado(Guid PedidoId, Guid ClienteId, Money Total) : IDomainEvent
    {
        public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
    }
}
