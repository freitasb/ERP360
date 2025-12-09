using ERP360.Pedidos.Domain.Entities;
using ERP360.Pedidos.Domain.Enums;

namespace ERP360.Pedidos.Domain.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Deve_Criar_Pedido_Valido()
        {
            var clienteId = Guid.NewGuid();

            // Act
            var pedido = Pedido.CriarRascunho(clienteId);

            // Assert
            Assert.Equal(clienteId, pedido.ClienteId);
            Assert.Equal(StatusPedido.AguardandoPagamento, pedido.Status);
            Assert.Empty(pedido.Itens);
        }

    }
}