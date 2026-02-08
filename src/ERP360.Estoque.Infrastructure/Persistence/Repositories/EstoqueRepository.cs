using ERP360.Estoque.Application.Abstractions;
using ERP360.Estoque.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ERP360.Estoque.Infrastructure.Persistence.Repositories
{
    public sealed class EstoqueRepository : IEstoqueRepository
    {
        private readonly EstoqueDbContext _db;

        public EstoqueRepository(EstoqueDbContext db) => _db = db;

        public Task<EstoqueItem?> GetByProdutoIdAsync(Guid produtoId, CancellationToken ct)
            => _db.EstoqueItens.FirstOrDefaultAsync(x => x.ProdutoId == produtoId, ct);

        public async Task SaveAsync(EstoqueItem item, CancellationToken ct)
        {
            _db.EstoqueItens.Update(item);
            await _db.SaveChangesAsync(ct);
        }
    }
}
