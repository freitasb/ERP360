using ERP360.Pedidos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP360.Pedidos.Infrastructure.Persistence
{
    public sealed class PedidosDbContext : DbContext
    {
        public PedidosDbContext(DbContextOptions<PedidosDbContext> options) : base(options) { }

        public DbSet<Pedido> Pedidos => Set<Pedido>();
        public DbSet<PedidoItem> PedidoItens => Set<PedidoItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Aplica todas as configurações Fluent API deste assembly
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PedidosDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
