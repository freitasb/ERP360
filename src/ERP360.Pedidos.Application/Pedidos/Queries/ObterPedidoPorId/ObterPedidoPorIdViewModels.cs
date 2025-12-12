using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP360.Pedidos.Application.Pedidos.Queries.ObterPedidoPorId
{
    public sealed record PedidoItemDetalheViewModel(
    Guid ProdutoId,
    string NomeProduto,
    int Quantidade,
    decimal PrecoUnitario,
    decimal Subtotal);

    public sealed record PedidoDetalhesViewModel(
        Guid PedidoId,
        Guid ClienteId,
        decimal ValorTotal,
        string Status,
        DateTimeOffset DataCriacao,
        IReadOnlyList<PedidoItemDetalheViewModel> Itens);
}
