using ERP360.Pedidos.Application.Pedidos.Events;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP360.Pedidos.Infrastructure.Messaging.Consumers
{
    public sealed class PedidoPagoConsumer : IConsumer<PedidoPago>
    {
        public Task Consume(ConsumeContext<PedidoPago> context)
        {
            var msg = context.Message;

            Console.WriteLine($"[RABBITMQ] Recebi PedidoPago: PedidoId={msg.PedidoId}, ClienteId={msg.ClienteId}");

            foreach (var item in msg.Itens)
            {
                Console.WriteLine($"[RABBITMQ] Reservar item: ProdutoId={item.ProdutoId}, Qtd={item.Quantidade}");
            }

            return Task.CompletedTask;
        }
    }
}
