using ERP360.Pedidos.Application.Consumers;
using ERP360.Pedidos.Application.Pedidos.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP360.Pedidos.Infrastructure.Consumers
{
    public sealed class EstoquePedidoPagoConsumerFake : IPedidoPagoConsumer
    {
        public Task HandleAsync(PedidoPago evento, CancellationToken ct)
        {
            // Simulação de reserva de estoque
            foreach (var item in evento.Itens)
            {
                Console.WriteLine(
                    $"[ESTOQUE] Reservando {item.Quantidade} do produto {item.ProdutoId}"
                );
            }

            return Task.CompletedTask;
        }
    }

}
