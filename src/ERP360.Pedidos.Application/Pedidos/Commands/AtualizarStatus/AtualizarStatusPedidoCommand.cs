using ERP360.Pedidos.Application.Abstractions;
using ERP360.Pedidos.Application.Common;
using ERP360.Pedidos.Application.Pedidos.Commands.ConfirmarPagamento;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP360.Pedidos.Application.Pedidos.Commands.AtualizarStatus
{
    public sealed record AtualizarStatusPedidoCommand(Guid PedidoId, string NovoStatus) : IRequest<Result>;
}
