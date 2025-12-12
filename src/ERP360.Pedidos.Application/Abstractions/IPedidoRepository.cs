using ERP360.Pedidos.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP360.Pedidos.Application.Abstractions
{
    public interface IPedidoRepository
    {
        Task<Pedido?> GetByIdAsync(Guid pedidoId, CancellationToken cancellationToken);
        Task AddAsync(Pedido pedido, CancellationToken ct = default);
        Task UpdateAsync(Pedido pedido, CancellationToken ct = default);
    }
}
