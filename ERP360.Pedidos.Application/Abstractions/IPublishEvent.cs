using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP360.Pedidos.Application.Abstractions
{
    public interface IPublishEvent
    {
        Task PublishAsync<T>(T message, CancellationToken ct = default);
    }
}
