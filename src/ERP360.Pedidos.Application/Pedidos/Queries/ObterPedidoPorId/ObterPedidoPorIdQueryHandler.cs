using ERP360.Pedidos.Application.Abstractions;
using ERP360.Pedidos.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP360.Pedidos.Application.Pedidos.Queries.ObterPedidoPorId
{
    public sealed class ObterPedidoPorIdQueryHandler
    : IRequestHandler<ObterPedidoPorIdQuery, Result<PedidoDetalhesViewModel>>
    {
        private readonly IPedidoRepository _pedidoRepository;

        public ObterPedidoPorIdQueryHandler(IPedidoRepository pedidoRepository)
        {
            _pedidoRepository = pedidoRepository;
        }

        public async Task<Result<PedidoDetalhesViewModel>> Handle(
            ObterPedidoPorIdQuery request,
            CancellationToken cancellationToken)
        {
            // 1) Busca o agregado no repositório (Port de saída).
            var pedido = await _pedidoRepository
                .GetByIdAsync(request.PedidoId, cancellationToken);

            if (pedido is null)
            {
                // Não é responsabilidade da Application escolher HTTP 404,
                // ela só diz: "não achei".
                return Result<PedidoDetalhesViewModel>.Failure("Pedido não encontrado.");
            }

            // 2) Monta o ReadModel a partir do agregado de domínio.
            var itensVm = pedido.Itens
                .Select(i =>
                {
                    var preco = i.PrecoUnitario.Amount; // VO Money -> decimal
                    var subtotal = preco * i.Quantidade;

                    return new PedidoItemDetalheViewModel(
                        i.ProdutoId,
                        i.NomeProduto,
                        i.Quantidade,
                        preco,
                        subtotal);
                })
                .ToList();

            var valorTotal = itensVm.Sum(x => x.Subtotal);

            var vm = new PedidoDetalhesViewModel(
                pedido.PedidoId,
                pedido.ClienteId,
                valorTotal,
                pedido.Status.ToString(),
                pedido.DataCriacao,
                itensVm);

            return Result<PedidoDetalhesViewModel>.Success(vm);
        }
    }

}
