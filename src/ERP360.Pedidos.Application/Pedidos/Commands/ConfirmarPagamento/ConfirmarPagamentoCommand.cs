using ERP360.Pedidos.Application.Abstractions;
using ERP360.Pedidos.Application.Common;
using ERP360.Pedidos.Application.Pedidos.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP360.Pedidos.Application.Pedidos.Commands.ConfirmarPagamento
{
    public sealed record ConfirmarPagamentoCommand(Guid PedidoId, string IdempotencyKey);


    public sealed class ConfirmarPagamentoCommandHandler
    {
        private readonly IPedidoRepository _repo;
        private readonly IPublishEvent _bus;


        public ConfirmarPagamentoCommandHandler(IPedidoRepository repo, IPublishEvent bus)
        { _repo = repo; _bus = bus; }


        public async Task<Result> Handle(ConfirmarPagamentoCommand cmd, CancellationToken ct = default)
        {
            // Idempotência: aqui apenas deixamos o campo; a logística real virá ao integrar com storage/cache.
            var pedido = await _repo.GetByIdAsync(cmd.PedidoId, ct);
            if (pedido is null) return Result.Failure("Pedido não encontrado.");


            pedido.MarcarPago(); // domínio garante transição válida


            await _repo.UpdateAsync(pedido, ct);


            // Publica evento de integração com itens solicitados (para Estoque)
            var itens = pedido.Itens.Select(i => new ItemSolicitado(i.ProdutoId, i.Quantidade)).ToList();
            var @event = new PedidoPago(pedido.PedidoId, pedido.ClienteId, itens);
            await _bus.PublishAsync(@event, ct);


            return Result.Success();
        }
    }
}
