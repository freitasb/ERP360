using ERP360.Pedidos.Domain.Entities;
using ERP360.Pedidos.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP360.Pedidos.Domain.Tests
{
    public sealed class PedidoStatusTests
    {
        [Fact]
        public void AlterarStatus_Deve_Mudar_Status_Quando_Transicao_For_Valida()
        {
            //Arrange - Criar o pedido com status passando de Rascunho para AguardandoPagamento
            var pedido = Pedido.CriarRascunho(Guid.NewGuid());
            pedido.Confirmar();

            //Act - Altero o status de forma válida
            var result = pedido.AlterarStatus(StatusPedido.Pago);

            //Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(StatusPedido.Pago, pedido.Status);
        }

        [Fact]
        public void AlterarStatus_Deve_Falhar_Quando_Transicao_For_InValida()
        {
            //Arrange - Criar o pedido com status Rascunho
            var pedido = Pedido.CriarRascunho(Guid.NewGuid());

            //Act - Altero o status de forma inválida
            var result = pedido.AlterarStatus(StatusPedido.Enviado);

            //Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(StatusPedido.Rascunho, pedido.Status);
            Assert.False(string.IsNullOrWhiteSpace(result.Error));
        }
    }
}
