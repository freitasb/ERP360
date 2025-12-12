using ERP360.Pedidos.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP360.Pedidos.Application.Pedidos.Queries.ObterPedidoPorId
{
   public sealed record ObterPedidoPorIdQuery(Guid PedidoId)
    : IRequest<Result<PedidoDetalhesViewModel>>;
}
