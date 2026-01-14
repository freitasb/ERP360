using ERP360.Contracts.Pedidos;
using ERP360.Pedidos.Application.Abstractions;
using ERP360.Pedidos.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP360.Pedidos.Application.Pedidos.Commands.ConfirmarPagamento
{
    public sealed record ConfirmarPagamentoCommand(Guid PedidoId) : IRequest<Result>;
}
