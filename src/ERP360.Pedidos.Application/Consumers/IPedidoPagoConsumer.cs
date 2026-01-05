using ERP360.Pedidos.Application.Pedidos.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP360.Pedidos.Application.Consumers
{
    public interface IPedidoPagoConsumer
    {
        Task HandleAsync(PedidoPago evento, CancellationToken ct);
    }

}
