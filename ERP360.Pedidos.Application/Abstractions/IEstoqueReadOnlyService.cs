using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP360.Pedidos.Application.Abstractions
{
    public interface IEstoqueReadOnlyService
    {
        Task<bool> CheckDisponibilidadeAsync(IEnumerable<ItemSolicitado> itens, CancellationToken ct = default);
    }

    public sealed record ItemSolicitado(Guid ProdutoId, int Quantidade);
}
