using ERP360.Contracts.Pedidos;
using ERP360.Pedidos.Application.Abstractions;
using ERP360.Pedidos.Application.Pedidos.Commands.ConfirmarPagamento;
using ERP360.Pedidos.Domain.Entities;
using ERP360.Pedidos.Domain.ValueObjects;
using Moq;

namespace ERP360.Estoque.Application.Tests
{
    public class ConfirmarPagamentoCommandHandlerTests
    {
        [Fact]
        public async Task Handle_Deve_Atualizar_Pedido_E_Publicar_PedidoPago_Quando_Sucesso()
        {
            // =========================================
            // ARRANGE (montar o cenário / mundo fake)
            // =========================================

            // 1) Criamos mocks das dependências externas do Handler.
            //    - Repo: representa "banco", mas aqui é fake.
            //    - Bus: representa "mensageria", mas aqui é fake.
            var repoMock = new Mock<IPedidoRepository>(MockBehavior.Strict);
            var busMock = new Mock<IPublishEvent>(MockBehavior.Strict);

            // 2) Criamos um Pedido REAL do domínio.
            //    (No unit test, domínio é real; o que é fake são as dependências externas)
            var pedido = Pedido.CriarRascunho(Guid.NewGuid());

            // O handler publica evento com base nos Itens do Pedido.
            // Então precisamos adicionar pelo menos 1 item real.
            var produtoId = Guid.NewGuid();
            pedido.AdicionarItem(produtoId, "Produto X", 2, Money.From(10m));

            // Para poder marcar Pago, a transição precisa ser válida.
            // Seu domínio exige: Rascunho -> AguardandoPagamento (Confirmar) -> Pago (MarcarPago)
            pedido.Confirmar();

            // 3) Configuramos o comportamento esperado do Repo:
            //    Quando o handler chamar GetByIdAsync com o PedidoId, deve devolver o pedido acima.
            repoMock
                .Setup(r => r.GetByIdAsync(pedido.PedidoId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(pedido);

            // 4) Configuramos o comportamento esperado do Repo ao atualizar:
            //    UpdateAsync deve ser chamado e retornar Task concluída.
            repoMock
                .Setup(r => r.UpdateAsync(pedido, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // 5) Configuramos o comportamento esperado do Bus:
            //    PublishAsync deve ser chamado com um PedidoPago que contenha o PedidoId correto
            //    e pelo menos 1 item (o ProdutoId e Quantidade que adicionamos).
            busMock
                .Setup(b => b.PublishAsync(
                    It.Is<PedidoPago>(e =>
                        e.PedidoId == pedido.PedidoId
                        && e.ClienteId == pedido.ClienteId
                        && e.Itens.Count == 1
                        && e.Itens[0].ProdutoId == produtoId
                        && e.Itens[0].Quantidade == 2
                    ),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // 6) Criamos a "unidade sob teste" (o Handler) com os mocks.
            var handler = new ConfirmarPagamentoCommandHandler(repoMock.Object, busMock.Object);

            // 7) Criamos o command (isso é o request do CQRS).
            var cmd = new ConfirmarPagamentoCommand(pedido.PedidoId);

            // =========================================
            // ACT (executar a unidade)
            // =========================================
            var result = await handler.Handle(cmd, CancellationToken.None);

            // =========================================
            // ASSERT (provar o que aconteceu)
            // =========================================

            // A) O handler deve retornar sucesso.
            Assert.True(result.IsSuccess);

            // B) Verificamos se o repo foi chamado exatamente como esperamos.
            repoMock.Verify(r => r.GetByIdAsync(pedido.PedidoId, It.IsAny<CancellationToken>()), Times.Once);
            repoMock.Verify(r => r.UpdateAsync(pedido, It.IsAny<CancellationToken>()), Times.Once);

            // C) Verificamos se o evento foi publicado exatamente 1 vez.
            busMock.Verify(b => b.PublishAsync(It.IsAny<PedidoPago>(), It.IsAny<CancellationToken>()), Times.Once);

            // D) Strict mocks: garante que não teve chamada “surpresa”.
            repoMock.VerifyNoOtherCalls();
            busMock.VerifyNoOtherCalls();

        }
    }
}