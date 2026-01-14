using ERP360.Estoque.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP360.Estoque.Application.Abstractions
{
    public interface IEstoqueRepository
    {
        Task<EstoqueItem?> GetByProdutoIdAsync(Guid produtoId, CancellationToken ct = default);
        Task SaveAsync(EstoqueItem item, CancellationToken ct = default);
    }
}
