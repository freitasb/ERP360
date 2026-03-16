using ERP360.Contracts.Pedidos;
using ERP360.Pedidos.Application.Abstractions;
using ERP360.Pedidos.Application.Pedidos.Commands.ConfirmarPagamento;
using ERP360.Pedidos.Domain.Entities;
using ERP360.Pedidos.Domain.ValueObjects;
using Moq;

namespace ERP360.Pedidos.Application.Tests
{
    public class ConfirmarPagamentoCommandHandlerTests
    {
        // Helpers: cria um Pedido “válido para pagar”
        // (Rascunho -> Adiciona item -> Confirmar() => AguardandoPagamento)
        private static Pedido CriarPedidoAguardandoPagamento(Guid? pedidoIdOverride = null)
        {
            var clienteId = Guid.NewGuid();
            var pedido = Pedido.CriarRascunho(clienteId);

            // Produto e item
            var produtoId = Guid.Parse("26222223-2221-2222-2222-222222222250");
            pedido.AdicionarItem(produtoId, "Produto X", 2, Money.From(10m));

            // Sai do rascunho e vai para AguardandoPagamento
            pedido.Confirmar();

            // Se você quiser forçar um ID específico, só pra facilitar algum cenário:
            // (só faça isso se sua entidade permitir; se não permitir, ignore esse override)
            if (pedidoIdOverride.HasValue)
            {
                // Se PedidoId não tiver setter acessível, NÃO faça isso.
                // Aqui fica só como “gancho mental”, sem assumir que seu domínio permite.
            }

            return pedido;
        }


        [Fact]
        public async Task Handle_Deve_Atualizar_Pedido_E_Publicar_PedidoPago_Quando_Sucesso()//verificar
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
            Assert.Null(result.Error);
            Assert.Equal(ERP360.Pedidos.Domain.Enums.StatusPedido.Pago, pedido.Status);

            // B) Verificamos se o repo foi chamado exatamente como esperamos.
            repoMock.Verify(r => r.GetByIdAsync(pedido.PedidoId, It.IsAny<CancellationToken>()), Times.Once);
            repoMock.Verify(r => r.UpdateAsync(pedido, It.IsAny<CancellationToken>()), Times.Once);

            // C) Verificamos se o evento foi publicado exatamente 1 vez.
            //busMock.Verify(b => b.PublishAsync(It.IsAny<PedidoPago>(), It.IsAny<CancellationToken>()), Times.Once);

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
        public async Task Handle_Deve_Retornar_Falha_Quando_Pedido_Nao_Existe_E_Nao_Publicar_Evento()
        {
            //ARANGE
            var pedidoId = Guid.NewGuid();
            var cmd = new ConfirmarPagamentoCommand(pedidoId);

            var repo = new Mock<IPedidoRepository>(MockBehavior.Strict);
            var bus = new Mock<IPublishEvent>(MockBehavior.Strict);

            repo.Setup(r => r.GetByIdAsync(pedidoId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Pedido?)null);

            var handler = new ConfirmarPagamentoCommandHandler(repo.Object, bus.Object);

            // ACT
            var result = await handler.Handle(cmd, CancellationToken.None);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Equal("Pedido não encontrado.", result.Error);

            // Não pode atualizar nem publicar
            repo.Verify(r => r.GetByIdAsync(pedidoId, It.IsAny<CancellationToken>()), Times.Once);
            repo.VerifyNoOtherCalls();
            bus.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Handle_Deve_Retornar_Falha_Quando_Dominio_Nao_Permite_Marcar_Pago()
        {
            // ARRANGE
            var pedido = CriarPedidoAguardandoPagamento();

            // Primeiro, marca pago “fora” do handler para simular que ele já estava pago
            var primeiroPagamento = pedido.MarcarPago();
            Assert.True(primeiroPagamento.IsSuccess);

            var cmd = new ConfirmarPagamentoCommand(pedido.PedidoId);

            var repo = new Mock<IPedidoRepository>(MockBehavior.Strict);
            var bus = new Mock<IPublishEvent>(MockBehavior.Strict);

            repo.Setup(r => r.GetByIdAsync(cmd.PedidoId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(pedido);

            var handler = new ConfirmarPagamentoCommandHandler(repo.Object, bus.Object);

            // ACT
            var result = await handler.Handle(cmd, CancellationToken.None);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Error);
            //Assert.Contains("Transição inválida", result.Error, StringComparison.OrdinalIgnoreCase);

            // Não deve atualizar nem publicar
            repo.Verify(r => r.GetByIdAsync(cmd.PedidoId, It.IsAny<CancellationToken>()), Times.Once);
            repo.VerifyNoOtherCalls();
            bus.VerifyNoOtherCalls();
        }
    }
}