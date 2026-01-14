using ERP360.Estoque.Application.Abstractions;
using ERP360.Estoque.Application.Common;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP360.Estoque.Application.Reservas.Command.ReservarEstoqueDoPedido
{
    public sealed class ReservarEstoqueDoPedidoCommandHandler
    : IRequestHandler<ReservarEstoqueDoPedidoCommand, Result>
    {
        private readonly IEstoqueRepository _repo;
        private readonly ILogger<ReservarEstoqueDoPedidoCommandHandler> _logger;

        public ReservarEstoqueDoPedidoCommandHandler(
            IEstoqueRepository repo,
            ILogger<ReservarEstoqueDoPedidoCommandHandler> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<Result> Handle(ReservarEstoqueDoPedidoCommand request, CancellationToken ct)
        {
            // 1) Guarda de entrada (Application-level)
            if (request.Itens is null || request.Itens.Count == 0)
                return Result.Failure("PedidoPago veio sem itens para reservar.");

            _logger.LogInformation("[ESTOQUE] Iniciando reserva do PedidoId={PedidoId}", request.PedidoId);

            foreach (var item in request.Itens)
            {
                if (item.Quantidade <= 0)
                    return Result.Failure($"Quantidade inválida para ProdutoId={item.ProdutoId}.");

                var estoqueItem = await _repo.GetByProdutoIdAsync(item.ProdutoId, ct);

                if (estoqueItem is null)
                    return Result.Failure($"ProdutoId={item.ProdutoId} não existe no estoque.");

                // 2) Regra de domínio (não damos throw como fluxo)
                if (!estoqueItem.PodeReservar(item.Quantidade))
                    return Result.Failure($"Estoque insuficiente para ProdutoId={item.ProdutoId}. Disponível={estoqueItem.QuantidadeDisponivel}, Solicitado={item.Quantidade}");

                estoqueItem.Reservar(item.Quantidade);

                // 3) Persistência
                await _repo.SaveAsync(estoqueItem, ct);

                _logger.LogInformation(
                    "[ESTOQUE] Reservado ProdutoId={ProdutoId} Qtd={Qtd} DisponívelAgora={Disponivel}",
                    item.ProdutoId, item.Quantidade, estoqueItem.QuantidadeDisponivel);
            }

            _logger.LogInformation("[ESTOQUE] Reserva concluída. PedidoId={PedidoId}", request.PedidoId);
            return Result.Success();
        }
    }
}
