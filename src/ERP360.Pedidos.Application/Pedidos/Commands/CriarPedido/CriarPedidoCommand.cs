using ERP360.Pedidos.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP360.Pedidos.Application.Pedidos.Commands.CriarPedido
{
    public sealed record CriarPedidoCommand(
    Guid ClienteId,
    IReadOnlyList<CriarPedidoItemCommand> Itens) : IRequest<Result<Guid>>;

    public sealed record CriarPedidoItemCommand(
    Guid ProdutoId,
    string NomeProduto,
    int Quantidade,
    decimal PrecoUnitario);
}
