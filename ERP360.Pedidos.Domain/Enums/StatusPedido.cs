using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP360.Pedidos.Domain.Enums
{
    public enum StatusPedido
    {
        Rascunho = 0,
        AguardandoPagamento = 1,
        Pago = 2,
        EmSeparacao = 3,
        Enviado = 4,
        Entregue = 5,
        EmDevolucao = 6,
        Devolvido = 7,
        Cancelado = 8
    }
}
