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

        public Task<Pedido?> GetByIdAsync(Guid pedidoId, CancellationToken cancellationToken)
        {
            var pedido = _store.FirstOrDefault(p => p.Value.PedidoId == pedidoId);

            // Se não encontrou nada, entry será um KeyValuePair padrão
            if (pedido.Equals(default(KeyValuePair<Guid, Pedido>)))
            {
                return Task.FromResult<Pedido?>(null);
            }
            return Task.FromResult<Pedido?>(pedido.Value);
        }


        public Task AddAsync(Pedido pedido, CancellationToken ct = default)
        { _store[pedido.PedidoId] = pedido; return Task.CompletedTask; }


        public Task UpdateAsync(Pedido pedido, CancellationToken ct = default)
        { _store[pedido.PedidoId] = pedido; return Task.CompletedTask; }
    }
}
