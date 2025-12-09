using ERP360.Pedidos.Application.Abstractions;
using ERP360.Pedidos.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP360.Pedidos.Infrastructure.InMemory
{
    public sealed class PedidoRepositoryInMemory : IPedidoRepository
    {
        private readonly Dictionary<Guid, Pedido> _store = new();


        public Task<Pedido?> GetAsync(Guid id, CancellationToken ct = default)
        => Task.FromResult(_store.TryGetValue(id, out var p) ? p : null);


        public Task AddAsync(Pedido pedido, CancellationToken ct = default)
        { _store[pedido.PedidoId] = pedido; return Task.CompletedTask; }


        public Task UpdateAsync(Pedido pedido, CancellationToken ct = default)
        { _store[pedido.PedidoId] = pedido; return Task.CompletedTask; }
    }
}
