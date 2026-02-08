using ERP360.Estoque.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP360.Estoque.Infrastructure.Persistence.Configurations
{
    public sealed class EstoqueItemConfiguration : IEntityTypeConfiguration<EstoqueItem>
    {
        public void Configure(EntityTypeBuilder<EstoqueItem> builder)
        {
            builder.ToTable("EstoqueItens");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ProdutoId)
                .IsRequired();

            builder.HasIndex(x => x.ProdutoId)
                .IsUnique();

            builder.Property(x => x.QuantidadeDisponivel)
                .IsRequired();
        }
    }
}
