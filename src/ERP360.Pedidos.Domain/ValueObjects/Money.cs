using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP360.Pedidos.Domain.ValueObjects
{
    public readonly record struct Money(decimal Amount, string Currency = "BRL")
    {
        public static Money Zero(string currency = "BRL") => new(0m, currency);
        public static Money From(decimal amount, string currency = "BRL")
        {
            if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount), "Valor não pode ser negativo.");
            return new Money(decimal.Round(amount, 2, MidpointRounding.AwayFromZero), currency);
        }
        public Money Add(Money other)
        {
            if (Currency != other.Currency) throw new InvalidOperationException("Moedas diferentes.");
            return new Money(Amount + other.Amount, Currency);
        }
        public Money Multiply(int qty) => new(Amount * qty, Currency);
        public override string ToString() => $"{Currency} {Amount:N2}";
    }
}
