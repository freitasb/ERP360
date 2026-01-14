using ERP360.Estoque.Application.Abstractions;
using ERP360.Estoque.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP360.Estoque.Infrastructure.Persistence.InMemory
{
    public sealed class EstoqueRepositoryInMemory : IEstoqueRepository
    {
        private readonly Dictionary<Guid, EstoqueItem> _store = new();

        public EstoqueRepositoryInMemory()
        {
            // Seed TEMP para conseguir testar já
            // (Depois vira EF + SQL e isso some)
            var p1 = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var p2 = Guid.Parse("22222222-2222-2222-2222-222222222222");

            _store[p1] = new EstoqueItem(p1, 10);
            _store[p2] = new EstoqueItem(p2, 5);
        }

        public Task<EstoqueItem?> GetByProdutoIdAsync(Guid produtoId, CancellationToken ct = default)
            => Task.FromResult(_store.TryGetValue(produtoId, out var item) ? item : null);

        public Task SaveAsync(EstoqueItem item, CancellationToken ct = default)
        {
            _store[item.ProdutoId] = item;
            return Task.CompletedTask;
        }
    }
}
