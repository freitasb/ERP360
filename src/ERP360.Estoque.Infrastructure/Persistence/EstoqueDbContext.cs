using ERP360.Estoque.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ERP360.Estoque.Infrastructure.Persistence
{
    public sealed class EstoqueDbContext : DbContext
    {
        public EstoqueDbContext(DbContextOptions<EstoqueDbContext> options) : base(options) { }

        public DbSet<EstoqueItem> EstoqueItens => Set<EstoqueItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(EstoqueDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
