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
        public Guid ProdutoId { get; private set; }
        public string NomeProduto { get; private set; }
        public int Quantidade { get; private set; }
        public Money PrecoUnitario { get; private set; }


        public Money Subtotal => PrecoUnitario.Multiply(Quantidade);


        public PedidoItem(Guid produtoId, string nomeProduto, int quantidade, Money precoUnitario)
        {
            if (produtoId == Guid.Empty) throw new ArgumentException("ProdutoId inválido.");
            if (string.IsNullOrWhiteSpace(nomeProduto)) throw new ArgumentException("Nome do produto é obrigatório.");
            if (quantidade <= 0) throw new ArgumentOutOfRangeException(nameof(quantidade), "Quantidade deve ser > 0.");
            if (precoUnitario.Amount <= 0) throw new ArgumentOutOfRangeException(nameof(precoUnitario), "Preço deve ser > 0.");


            ProdutoId = produtoId;
            NomeProduto = nomeProduto.Trim();
            Quantidade = quantidade;
            PrecoUnitario = Money.From(precoUnitario.Amount, precoUnitario.Currency);
        }
    }
}
