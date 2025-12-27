using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP360.Pedidos.Domain.ValueObjects
{
    public readonly record struct Money(decimal Amount)
    {
        public static Money Zero() => new(0m);

        public static Money From(decimal amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "Valor não pode ser negativo.");

            return new Money(decimal.Round(amount, 2, MidpointRounding.AwayFromZero));
        }

        public Money Add(Money other) => new(Amount + other.Amount);

        public Money Multiply(int qty) => new(Amount * qty);

        public override string ToString() => $"BRL {Amount:N2}";
    }
}
