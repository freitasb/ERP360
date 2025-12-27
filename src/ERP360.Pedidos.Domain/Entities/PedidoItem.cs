using ERP360.Pedidos.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP360.Pedidos.Domain.Entities
{
    public sealed class PedidoItem
    {
        public Guid Id { get; private set; }
        public Guid PedidoId { get; private set; }

        public Guid ProdutoId { get; private set; }
        public string NomeProduto { get; private set; } = null!;
        public int Quantidade { get; private set; }
        public Money PrecoUnitario { get; private set; }

        public Money Subtotal => PrecoUnitario.Multiply(Quantidade);

        private PedidoItem() { } // EF

        internal PedidoItem(
            Guid pedidoId,
            Guid produtoId,
            string nomeProduto,
            int quantidade,
            Money precoUnitario)
        {
            Id = Guid.NewGuid();
            PedidoId = pedidoId;

            if (produtoId == Guid.Empty)
                throw new ArgumentException("ProdutoId inválido.");

            if (string.IsNullOrWhiteSpace(nomeProduto))
                throw new ArgumentException("Nome do produto é obrigatório.");

            if (quantidade <= 0)
                throw new ArgumentOutOfRangeException(nameof(quantidade));

            if (precoUnitario.Amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(precoUnitario));

            ProdutoId = produtoId;
            NomeProduto = nomeProduto.Trim();
            Quantidade = quantidade;
            PrecoUnitario = precoUnitario;
        }
    }

}
