using ERP360.Contracts.Pedidos;
using ERP360.Estoque.Application.Reservas.Command.ReservarEstoqueDoPedido;
using MassTransit;
using MediatR;

namespace ERP360.Estoque.Api.Messaging.Consumers
{
    public sealed class PedidoPagoConsumer : IConsumer<PedidoPago>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<PedidoPagoConsumer> _logger;

        public PedidoPagoConsumer(IMediator mediator, ILogger<PedidoPagoConsumer> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PedidoPago> context)
        {
            var msg = context.Message;

            _logger.LogInformation("[ESTOQUE] Recebi PedidoPago. PedidoId={PedidoId} Itens={QtdItens}",
                msg.PedidoId, msg.Itens.Count);

            var itens = msg.Itens
                .Select(i => new ReservarItemCommand(i.ProdutoId, i.Quantidade))
                .ToList();

            var cmd = new ReservarEstoqueDoPedidoCommand(msg.PedidoId, itens);

            var result = await _mediator.Send(cmd, context.CancellationToken);

            if (!result.IsSuccess)
            {
                // ✅ ACK + LOG (não derruba o canal)
                _logger.LogWarning("[ESTOQUE] Falha ao reservar estoque. PedidoId={PedidoId}. Erro={Erro}",
                    msg.PedidoId, result.Error);

                return;
            }
            else
            {
                _logger.LogInformation(
                    "[ESTOQUE] Reserva concluída com sucesso. PedidoId={PedidoId}",
                    msg.PedidoId);
            }
        }
    }
}
