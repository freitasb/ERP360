using ERP360.Pedidos.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP360.Pedidos.Domain.Events
{
    public sealed record PedidoCancelado(Guid PedidoId, string Motivo) : IDomainEvent
    {
        public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
    }
}
