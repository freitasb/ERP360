using ERP360.Pedidos.Application.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP360.Pedidos.Application.Pedidos.Events
{
    public sealed record PedidoPago(Guid PedidoId, Guid ClienteId, IReadOnlyList<ItemSolicitado> Itens);
}
