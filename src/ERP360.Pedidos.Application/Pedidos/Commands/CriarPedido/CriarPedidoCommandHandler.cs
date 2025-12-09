using ERP360.Pedidos.Application.Abstractions;
using ERP360.Pedidos.Application.Common;
using ERP360.Pedidos.Domain.Entities;
using ERP360.Pedidos.Domain.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP360.Pedidos.Application.Pedidos.Commands.CriarPedido
{
    public sealed class CriarPedidoCommandHandler : IRequestHandler<CriarPedidoCommand, Result<Guid>>
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IEstoqueReadOnlyService _estoqueReadOnly;

        public CriarPedidoCommandHandler(IPedidoRepository pedidoRepository, IEstoqueReadOnlyService estoqueReadOnly)
        {
            _pedidoRepository = pedidoRepository;
            _estoqueReadOnly = estoqueReadOnly;
        }

        public async Task<Result<Guid>>Handle(CriarPedidoCommand request, CancellationToken cancellationToken)
        {
            // 1) Validações básicas de orquestração (não de domínio).
            //if (request.Itens is null || request.Itens.Count == 0)
                //return Result<Guid>.Failure("O pedido deve conter pelo menos um item.");

            //if (request.Itens.Any(i => i.Quantidade <= 0 || i.PrecoUnitario <= 0))
                //return Result<Guid>.Failure("Itens precisam ter quantidade e preço maiores que zero.");

            // 2) Pré-check de disponibilidade de estoque (síncrono e rápido).
            // Fundamentos: Ports & Adapters + separação de domínios (Pedidos x Estoque).
            var itensSolicitados = request.Itens
                .Select(i => new ItemSolicitado(i.ProdutoId, i.Quantidade))
                .ToList();

            var disponivel = await _estoqueReadOnly
                .CheckDisponibilidadeAsync(itensSolicitados, cancellationToken);

            if (!disponivel)
                return Result<Guid>.Failure("Estoque insuficiente para um ou mais itens.");

            // 3) Criação do agregado de domínio.
            // Fundamentos: DDD (Agregado Pedido controla consistência dos itens e status).
            var pedido = Pedido.CriarRascunho(request.ClienteId);

            foreach (var item in request.Itens)
            {
                pedido.AdicionarItem(
                    item.ProdutoId,
                    item.NomeProduto,
                    item.Quantidade,
                    Money.From(item.PrecoUnitario));
            }

            // Transição de estado de Rascunho -> AguardandoPagamento
            // e emissão do Domain Event PedidoCriado (implementado no domínio).
            pedido.Confirmar();

            // 4) Persistência via porta abstrata (Repository).
            // Fundamentos: Repository Pattern + Ports & Adapters.
            await _pedidoRepository.AddAsync(pedido, cancellationToken);

            // 5) Retorno do resultado com o Id do pedido.
            return Result<Guid>.Success(pedido.PedidoId);
        }
    }
}
