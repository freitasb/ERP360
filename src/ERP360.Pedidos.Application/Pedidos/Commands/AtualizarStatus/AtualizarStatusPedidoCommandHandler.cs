using ERP360.Contracts.Pedidos;
using ERP360.Pedidos.Application.Abstractions;
using ERP360.Pedidos.Application.Common;
using ERP360.Pedidos.Application.Pedidos.Events;
using ERP360.Pedidos.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ERP360.Pedidos.Application.Pedidos.Commands.AtualizarStatus
{
    public sealed class AtualizarStatusPedidoCommandHandler : IRequestHandler<AtualizarStatusPedidoCommand, Result>
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IPublishEvent _bus;

        public AtualizarStatusPedidoCommandHandler(IPedidoRepository pedidoRepository, IPublishEvent bus)
        {
            _pedidoRepository = pedidoRepository;
            _bus = bus;
        }

        public async Task<Result> Handle(AtualizarStatusPedidoCommand cmd, CancellationToken ct = default)
        {
            var pedido = await _pedidoRepository.GetByIdAsync(cmd.PedidoId, ct);
            if (pedido is null) return Result.Failure("Pedido não encontrado.");

            if (!Enum.TryParse<StatusPedido>(cmd.NovoStatus, true, out var destino))
                return Result.Failure("Status inválido.");

            var domainResult = pedido.AlterarStatus(destino);

            if (!domainResult.IsSuccess)
                return Result.Failure(domainResult.Error);

            await _pedidoRepository.UpdateAsync(pedido, ct);

            // ✅ Gatilho temporário de aprendizado (até existir ConfirmarPagamento)
            if (destino == StatusPedido.Pago)
            {
                var itens = pedido.Itens.Select(i => new ItemSolicitado(i.ProdutoId, i.Quantidade)).ToList();
                var @event = new PedidoPago(pedido.PedidoId, pedido.ClienteId, itens);
                await _bus.PublishAsync(@event, ct);
            }

            return Result.Success();
        }
    }
}
