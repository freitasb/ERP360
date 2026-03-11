using ERP360.Contracts.Pedidos;
using ERP360.Pedidos.Application.Abstractions;
using ERP360.Pedidos.Application.Pedidos.Commands.AtualizarStatus;
using ERP360.Pedidos.Domain.Entities;
using ERP360.Pedidos.Domain.Enums;
using ERP360.Pedidos.Domain.ValueObjects;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP360.Pedidos.Application.Tests
{
    public class AtualizarStatusPedidoCommandHandlerTests
    {
        [Fact]
        public async Task Handle_Deve_Atualizar_Pedido()
        {
            var repoMock = new Mock<IPedidoRepository>(MockBehavior.Strict);
            var busMock = new Mock<IPublishEvent>(MockBehavior.Strict);

            var pedido = Pedido.CriarRascunho(Guid.NewGuid());

            var produtoId = Guid.NewGuid();
            pedido.AdicionarItem(produtoId, "Jaca", 3, Money.From(15m));

            repoMock.Setup(r => r.GetByIdAsync(pedido.PedidoId, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(pedido);

            repoMock.Setup(r => r.UpdateAsync(pedido, It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

            var handler = new AtualizarStatusPedidoCommandHandler(repoMock.Object, busMock.Object);
            var cmd = new AtualizarStatusPedidoCommand(pedido.PedidoId, StatusPedido.AguardandoPagamento.ToString());

            var result = await handler.Handle(cmd);

            Assert.True(result.IsSuccess);
            Assert.Null(result.Error);

            repoMock.Verify(r => r.GetByIdAsync(pedido.PedidoId, It.IsAny<CancellationToken>()), Times.Once);
            repoMock.Verify(r => r.UpdateAsync(pedido, It.IsAny<CancellationToken>()), Times.Once);
            repoMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Handle_Deve_Atualizar_E_Publicar_PedidoPago_Quando_Status_For_Pago()
        {
            var repoMock = new Mock<IPedidoRepository>(MockBehavior.Strict);
            var busMock = new Mock<IPublishEvent>(MockBehavior.Strict);
        
            var pedido = Pedido.CriarRascunho(Guid.NewGuid());
        
            var produtoId = Guid.NewGuid();
            pedido.AdicionarItem(produtoId, "Jaca", 3, Money.From(15m));

            var pedidoAguardado = pedido.Confirmar();
            Assert.True(pedidoAguardado.IsSuccess);

            repoMock.Setup(r => r.GetByIdAsync(pedido.PedidoId, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(pedido);
        
            repoMock.Setup(r => r.UpdateAsync(pedido, It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

            busMock
                .Setup(b => b.PublishAsync(
                    It.Is<PedidoPago>(e =>
                        e.PedidoId == pedido.PedidoId
                        && e.ClienteId == pedido.ClienteId
                        && e.Itens.Count == 1
                        && e.Itens[0].ProdutoId == produtoId
                        && e.Itens[0].Quantidade == 3
                    ),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var handler = new AtualizarStatusPedidoCommandHandler(repoMock.Object, busMock.Object);
            var cmd = new AtualizarStatusPedidoCommand(pedido.PedidoId, StatusPedido.Pago.ToString());
        
            var result = await handler.Handle(cmd);

            Assert.True(result.IsSuccess);
            Assert.Null(result.Error);
            repoMock.Verify(r => r.GetByIdAsync(pedido.PedidoId, It.IsAny<CancellationToken>()), Times.Once);
            repoMock.Verify(r => r.UpdateAsync(pedido, It.IsAny<CancellationToken>()), Times.Once);

            busMock.Verify(b => b.PublishAsync(
                It.Is<PedidoPago>(e =>
                    e.PedidoId == pedido.PedidoId &&
                    e.ClienteId == pedido.ClienteId &&
                    e.Itens.Count == pedido.Itens.Count &&
                    e.Itens.All(itemEvento =>
                        pedido.Itens.Any(itemPedido =>
                            itemPedido.ProdutoId == itemEvento.ProdutoId &&
                            itemPedido.Quantidade == itemEvento.Quantidade
                        )
                    )
                ),
                It.IsAny<CancellationToken>()),
                Times.Once);

            // D) Strict mocks: garante que não teve chamada “surpresa”.
            repoMock.VerifyNoOtherCalls();
            busMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Handle_Deve_Falhar_Quando_Pedido_Nao_Encontrado()
        {
            var ct = CancellationToken.None;

            var pedidoId = Guid.NewGuid();
            var cmd = new AtualizarStatusPedidoCommand(pedidoId, "Pago");

            var repoMock = new Mock<IPedidoRepository>(MockBehavior.Strict);
            var busMock = new Mock<IPublishEvent>(MockBehavior.Strict);

            repoMock.Setup(r => r.GetByIdAsync(pedidoId, It.IsAny<CancellationToken>()))
                    .ReturnsAsync((Pedido?)null);

            var handler = new AtualizarStatusPedidoCommandHandler(repoMock.Object, busMock.Object);

            var result = await handler.Handle(cmd, ct);

            Assert.False(result.IsSuccess);
            Assert.Equal("Pedido não encontrado.", result.Error);

            repoMock.Verify(r => r.GetByIdAsync(pedidoId, It.IsAny<CancellationToken>()), Times.Once);
            repoMock.VerifyNoOtherCalls();
            busMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Handle_Deve_Falhar_Quando_Status_For_Invalido()
        {
            var ct = CancellationToken.None;

            //var pedido = CriarPedidoAguardandoPagamentoComItem(out _);

            var pedido = Pedido.CriarRascunho(Guid.NewGuid());
            var produtoId = Guid.NewGuid();

            pedido.AdicionarItem(produtoId, "Produto X", 2, Money.From(10m));
            pedido.Confirmar();

            var cmd = new AtualizarStatusPedidoCommand(pedido.PedidoId, "NaoExisteEsseStatus");

            var repoMock = new Mock<IPedidoRepository>(MockBehavior.Strict);
            var busMock = new Mock<IPublishEvent>(MockBehavior.Strict);

            repoMock.Setup(r => r.GetByIdAsync(pedido.PedidoId, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(pedido);

            var handler = new AtualizarStatusPedidoCommandHandler(repoMock.Object, busMock.Object);

            var result = await handler.Handle(cmd, ct);

            Assert.False(result.IsSuccess);
            Assert.Equal("Status inválido.", result.Error);

            // não pode atualizar nem publicar
            repoMock.Verify(r => r.GetByIdAsync(pedido.PedidoId, It.IsAny<CancellationToken>()), Times.Once);
            repoMock.VerifyNoOtherCalls();
            busMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Handle_Deve_Falhar_Quando_Dominio_Nao_Permite_Transicao()
        {
            var ct = CancellationToken.None;

            // Pedido está em Rascunho (sem Confirmar) para forçar transição inválida
            var pedido = Pedido.CriarRascunho(Guid.NewGuid());

            var cmd = new AtualizarStatusPedidoCommand(pedido.PedidoId, "Enviado");

            var repoMock = new Mock<IPedidoRepository>(MockBehavior.Strict);
            var busMock = new Mock<IPublishEvent>(MockBehavior.Strict);

            repoMock.Setup(r => r.GetByIdAsync(pedido.PedidoId, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(pedido);

            var handler = new AtualizarStatusPedidoCommandHandler(repoMock.Object, busMock.Object);

            var result = await handler.Handle(cmd, ct);

            Assert.False(result.IsSuccess);
            Assert.False(string.IsNullOrWhiteSpace(result.Error));

            // não pode atualizar nem publicar
            repoMock.Verify(r => r.GetByIdAsync(pedido.PedidoId, It.IsAny<CancellationToken>()), Times.Once);
            repoMock.VerifyNoOtherCalls();
            busMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Handle_Nao_Deve_Publicar_Evento_Quando_Destino_Nao_For_Pago()
        {
            var repoMock = new Mock<IPedidoRepository>(MockBehavior.Strict);
            var busMock = new Mock<IPublishEvent>(MockBehavior.Strict);

            var pedido = Pedido.CriarRascunho(Guid.NewGuid());
            var produtoId = Guid.NewGuid();
            pedido.AdicionarItem(produtoId, "Jaca", 3, Money.From(15m));

            repoMock.Setup(r => r.GetByIdAsync(pedido.PedidoId, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(pedido);

            repoMock.Setup(r => r.UpdateAsync(pedido, It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

            var handler = new AtualizarStatusPedidoCommandHandler(repoMock.Object, busMock.Object);
            var cmd = new AtualizarStatusPedidoCommand(pedido.PedidoId, StatusPedido.AguardandoPagamento.ToString());

            var result = await handler.Handle(cmd);

            Assert.True(result.IsSuccess);
            Assert.Null(result.Error);

            repoMock.Verify(r => r.GetByIdAsync(pedido.PedidoId, It.IsAny<CancellationToken>()), Times.Once);
            repoMock.Verify(r => r.UpdateAsync(pedido, It.IsAny<CancellationToken>()), Times.Once);

            busMock.Verify(
                b => b.PublishAsync(It.IsAny<PedidoPago>(), It.IsAny<CancellationToken>()),
                Times.Never);

            repoMock.VerifyNoOtherCalls();
            busMock.VerifyNoOtherCalls();
        }
    }
}
