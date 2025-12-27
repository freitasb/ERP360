using ERP360.Pedidos.Domain.Entities;
using ERP360.Pedidos.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP360.Pedidos.Infrastructure.Persistence.Configurations
{
    public sealed class PedidoItemConfiguration : IEntityTypeConfiguration<PedidoItem>
    {
        public void Configure(EntityTypeBuilder<PedidoItem> builder)
        {
            builder.ToTable("PedidoItens");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.PedidoId).IsRequired();
            builder.Property(x => x.ProdutoId).IsRequired();

            builder.Property(x => x.NomeProduto)
                   .HasMaxLength(200)
                   .IsRequired();

            builder.Property(x => x.Quantidade)
                   .IsRequired();

            builder.Property(x => x.PrecoUnitario)
                   .HasConversion(
                       v => v.Amount,
                       v => Money.From(v))
                   .HasColumnName("PrecoUnitario")
                   .HasPrecision(18, 2)
                   .IsRequired();

            builder.HasOne<Pedido>()
                   .WithMany(p => p.Itens)
                   .HasForeignKey(x => x.PedidoId)
                   .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
