using ERP360.Pedidos.Application.Abstractions;
using ERP360.Pedidos.Application.Common;
using ERP360.Pedidos.Domain.Entities;
using ERP360.Pedidos.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP360.Pedidos.Application.Pedidos.Commands.ConfirmarPedido
{
    public sealed record ConfirmarPedidoCommand(Guid ClienteId, List<ItemSolicitadoInput> Itens);
    public sealed record ItemSolicitadoInput(Guid ProdutoId, string NomeProduto, int Quantidade, decimal PrecoUnitario);


    public sealed class ConfirmarPedidoCommandHandler
    {
        private readonly IPedidoRepository _repo;
        private readonly IEstoqueReadOnlyService _estoque;


        public ConfirmarPedidoCommandHandler(IPedidoRepository repo, IEstoqueReadOnlyService estoque)
        { _repo = repo; _estoque = estoque; }


        public async Task<Result<Guid>> Handle(ConfirmarPedidoCommand cmd, CancellationToken ct = default)
        {
            // 1) validar entrada básica (Application — validação de orquestração)
            if (cmd.Itens is null || cmd.Itens.Count == 0)
                return Result<Guid>.Failure("Pedido deve conter ao menos um item.");
            if (cmd.Itens.Any(i => i.Quantidade <= 0 || i.PrecoUnitario <= 0))
                return Result<Guid>.Failure("Itens com quantidade e preço > 0 são obrigatórios.");


            // 2) pré-check de disponibilidade (síncrono, rápido)
            var itensSolicitados = cmd.Itens.Select(i => new ItemSolicitado(i.ProdutoId, i.Quantidade)).ToList();
            var disponivel = await _estoque.CheckDisponibilidadeAsync(itensSolicitados, ct);
            if (!disponivel)
                return Result<Guid>.Failure("Estoque insuficiente para um ou mais itens.");


            // 3) construir o agregado e confirmar
            var pedido = Pedido.CriarRascunho(cmd.ClienteId);
            foreach (var i in cmd.Itens)
                pedido.AdicionarItem(i.ProdutoId, i.NomeProduto, i.Quantidade, Money.From(i.PrecoUnitario));


            pedido.Confirmar(); // Rascunho -> AguardandoPagamento (+ evento de domínio PedidoCriado)


            // 4) persistir (ainda in-memory)
            await _repo.AddAsync(pedido, ct);


            return Result<Guid>.Success(pedido.PedidoId);
        }
    }
}
