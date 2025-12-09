using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP360.Pedidos.Domain.Primitives
{
    public interface IDomainEvent
    {
        DateTimeOffset OccurredOn { get; }
    }
}
