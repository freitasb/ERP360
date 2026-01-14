using ERP360.Estoque.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP360.Estoque.Application.Reservas.Command.ReservarEstoqueDoPedido
{
    public sealed record ReservarEstoqueDoPedidoCommand(
    Guid PedidoId,
    IReadOnlyList<ReservarItemCommand> Itens
) : IRequest<Result>;

    public sealed record ReservarItemCommand(Guid ProdutoId, int Quantidade);
}
