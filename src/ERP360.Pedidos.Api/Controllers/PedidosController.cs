using ERP360.Pedidos.Api.Contracts.Pedidos;
using ERP360.Pedidos.Application.Pedidos.Commands.CriarPedido;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ERP360.Pedidos.Api.Controllers
{
    [ApiController]
    // Aqui definimos a rota base do controller:
    [Route("api/v1/pedidos")]
    public class PedidosController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PedidosController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("health")]
        public IActionResult Health()
        => Ok(new { status = "healthy", version = "v1", api = "Pedidos" });

        // POST /api/v1/pedidos
        // Fluxo: API -> DTO -> Command -> MediatR -> Handler -> Domain -> Repository.
        [HttpPost]
        public async Task<IActionResult> CriarPedido(
            [FromBody] CriarPedidoDto dto,
            CancellationToken cancellationToken)
        {
            // Mapeia DTO -> Command (camada API não conhece o domínio).
            var itensCommand = dto.Itens
                .Select(i => new CriarPedidoItemCommand(
                    i.ProdutoId,
                    i.NomeProduto,
                    i.Quantidade,
                    i.PrecoUnitario))
                .ToList();

            var command = new CriarPedidoCommand(
                dto.ClienteId,
                itensCommand);

            var result = await _mediator.Send(command, cancellationToken);

            if (!result.IsSuccess)
            {
                // Por enquanto, 400 simples com mensagem. Depois evoluímos para ProblemDetails.
                return BadRequest(new { error = result.Error });
            }

            var pedidoId = result.Value;

            // Location manual por enquanto. Depois, quando existir GET /pedidos/{id},
            // podemos usar Url.Action(nameof(ObterPorId), ...) se desejarmos.
            var location = $"/api/v1/pedidos/{pedidoId}";

            return Created(location, new { id = pedidoId });
        }
    }
}
