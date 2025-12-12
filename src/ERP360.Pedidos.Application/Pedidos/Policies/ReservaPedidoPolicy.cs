using ERP360.Pedidos.Application.Abstractions;
using ERP360.Pedidos.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP360.Pedidos.Application.Pedidos.Policies
{
    public sealed record ReservaConfirmada(Guid PedidoId);
    public sealed record ReservaFalhou(Guid PedidoId, string Motivo);


    public sealed class ReservaPedidoPolicy
    {
        private readonly IPedidoRepository _repo;
        public ReservaPedidoPolicy(IPedidoRepository repo) { _repo = repo; }


        public async Task<Result> Handle(ReservaConfirmada message, CancellationToken ct = default)
        {
            var pedido = await _repo.GetByIdAsync(message.PedidoId, ct);
            if (pedido is null) return Result.Failure("Pedido não encontrado.");


            pedido.IniciarSeparacao();
            await _repo.UpdateAsync(pedido, ct);
            return Result.Success();
        }


        public async Task<Result> Handle(ReservaFalhou message, CancellationToken ct = default)
        {
            var pedido = await _repo.GetByIdAsync(message.PedidoId, ct);
            if (pedido is null) return Result.Failure("Pedido não encontrado.");


            pedido.CancelarManual(); // encurta o fluxo para didática; mais à frente trataremos compensação explícita
            await _repo.UpdateAsync(pedido, ct);
            return Result.Success();
        }
    }
}
