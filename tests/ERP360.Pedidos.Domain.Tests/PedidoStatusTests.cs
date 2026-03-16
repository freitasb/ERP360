using ERP360.Pedidos.Domain.Entities;
using ERP360.Pedidos.Domain.Enums;
using ERP360.Pedidos.Domain.Events;
using ERP360.Pedidos.Domain.ValueObjects;
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

        [Fact]
        public void Confirmar_Deve_Mudar_Status_Para_AguardandoPagamento_E_Emitir_Eventos()
        {
            var pedido = Pedido.CriarRascunho(Guid.NewGuid());
            pedido.AdicionarItem(Guid.NewGuid(), "Produto X", 2, Money.From(10m));
            var result = pedido.Confirmar();

            Assert.True(result.IsSuccess);//Assegura que o processo foi um sucesso
            Assert.Equal(StatusPedido.AguardandoPagamento, pedido.Status);//Confirma o status atual do pedido
            Assert.Contains(pedido.Events, e => e is StatusPedidoAlterado);//Confirma que o status foi alterado
            Assert.Contains(pedido.Events, e => e is PedidoCriado);
        }

        [Fact]
        public void MarcarPago_Deve_Mudar_Status_Quando_Transicao_For_Valida()
        {
            var pedido = Pedido.CriarRascunho(Guid.NewGuid());
            pedido.Confirmar();
            var result = pedido.MarcarPago();

            Assert.True(result.IsSuccess);
            Assert.Equal(StatusPedido.Pago, pedido.Status);
            Assert.Contains(pedido.Events, e => e is StatusPedidoAlterado);
            //Assert.Contains(pedido.Events, e => e is PedidoCriado);

        }

        [Fact]
        public void MarcarPago_Deve_Falhar_Quando_Transicao_For_Invalida()
        {
            var pedido = Pedido.CriarRascunho(Guid.NewGuid());
            var result = pedido.MarcarPago();

            Assert.False(result.IsSuccess);
            Assert.Equal(StatusPedido.Rascunho, pedido.Status);
            Assert.False(string.IsNullOrWhiteSpace(result.Error));
        }

        [Fact]
        public void CancelarManual_Deve_Cancelar_Quando_Ainda_Nao_Foi_Enviado()
        {
            var pedido = Pedido.CriarRascunho(Guid.NewGuid());
            pedido.Confirmar();
            pedido.MarcarPago();

            var result = pedido.CancelarManual();

            Assert.True(result.IsSuccess);
            Assert.Equal(StatusPedido.Cancelado, pedido.Status);
            Assert.Contains(pedido.Events, e => e is PedidoCancelado);
            Assert.True(string.IsNullOrWhiteSpace(result.Error));
        }
    }
}
