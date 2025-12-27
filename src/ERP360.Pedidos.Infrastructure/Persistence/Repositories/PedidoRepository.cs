using ERP360.Pedidos.Application.Abstractions;
using ERP360.Pedidos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ERP360.Pedidos.Infrastructure.Persistence.Repositories
{
    public sealed class PedidoRepository : IPedidoRepository
    {
        private readonly PedidosDbContext _db;

        public PedidoRepository(PedidosDbContext db)
        {
           _db = db;
        }

        public async Task AddAsync(Pedido pedido, CancellationToken ct = default)
        {
            await _db.Pedidos.AddAsync(pedido, ct);
            await _db.SaveChangesAsync(ct);
        }

        public async Task<Pedido?> GetByIdAsync(Guid pedidoId, CancellationToken cancellationToken)
        {
            return await _db.Pedidos
            .Include(p => p.Itens)
            .FirstOrDefaultAsync(p => p.PedidoId == pedidoId, cancellationToken);
        }

        public Task UpdateAsync(Pedido pedido, CancellationToken ct = default)
        {
            _db.Pedidos.Update(pedido);
            return _db.SaveChangesAsync(ct);
        }
    }
}
