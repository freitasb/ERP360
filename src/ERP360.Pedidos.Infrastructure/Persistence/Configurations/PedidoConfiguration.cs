using ERP360.Pedidos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP360.Pedidos.Infrastructure.Persistence.Configurations
{
    public sealed class PedidoConfiguration : IEntityTypeConfiguration<Pedido>
    {
        public void Configure(EntityTypeBuilder<Pedido> builder)
        {
            builder.ToTable("Pedidos");

            builder.HasKey(x => x.PedidoId);

            builder.Property(x => x.ClienteId)
                   .IsRequired();

            builder.Property(x => x.DataCriacao)
                   .IsRequired();

            // Status (enum) -> string (debug-friendly e legível no banco)
            builder.Property(x => x.Status)
                   .HasConversion<string>()
                   .IsRequired();

            // Relacionamento 1:N
            builder.HasMany(x => x.Itens)
                   .WithOne()
                   .HasForeignKey("PedidoId")
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
    
}
