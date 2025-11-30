using ERP360.Pedidos.Domain.Enums;
using ERP360.Pedidos.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP360.Pedidos.Domain.Events
{
    public sealed record StatusPedidoAlterado(Guid PedidoId,
                                              StatusPedido De,
                                              StatusPedido Para,
                                              string Motivo,
                                              DateTimeOffset Data) : IDomainEvent
    {
        public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
    }
}
