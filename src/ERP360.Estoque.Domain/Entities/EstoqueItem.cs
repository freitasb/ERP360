using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP360.Estoque.Domain.Entities
{
    public sealed class EstoqueItem
    {
        public Guid ProdutoId { get; private set; }
        public int QuantidadeDisponivel { get; private set; }

        private EstoqueItem() { } // EF futuramente

        public EstoqueItem(Guid produtoId, int quantidadeDisponivel)
        {
            if (produtoId == Guid.Empty) throw new ArgumentException("ProdutoId inválido.");
            if (quantidadeDisponivel < 0) throw new ArgumentOutOfRangeException(nameof(quantidadeDisponivel));

            ProdutoId = produtoId;
            QuantidadeDisponivel = quantidadeDisponivel;
        }

        public bool PodeReservar(int quantidade)
        {
            return quantidade > 0 && QuantidadeDisponivel >= quantidade;
        }

        public void Reservar(int quantidade)
        {
            if (!PodeReservar(quantidade))
                throw new InvalidOperationException($"Estoque insuficiente para ProdutoId={ProdutoId}. Solicitado={quantidade} Disponível={QuantidadeDisponivel}");

            QuantidadeDisponivel -= quantidade;
        }
    }
}
