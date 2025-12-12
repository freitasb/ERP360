using ERP360.Pedidos.Api.Contracts.Pedidos.BuscarPedido;
using ERP360.Pedidos.Api.Contracts.Pedidos.CriarPedido;
using ERP360.Pedidos.Application.Pedidos.Commands.CriarPedido;
using ERP360.Pedidos.Application.Pedidos.Queries.ObterPedidoPorId;
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

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> ObterPorId(
        Guid id,
        CancellationToken cancellationToken)
        {
            // (a) transformar o HTTP em uma intenção de leitura (Query)
            var query = new ObterPedidoPorIdQuery(id);

            // (b) enviar essa intenção para a Application via MediatR
            var result = await _mediator.Send(query, cancellationToken);

            // (c) tratar erros e sucesso
            if (!result.IsSuccess)
            {
                if (result.Error == "Pedido não encontrado.")
                    return NotFound(new { error = result.Error });

                return BadRequest(new { error = result.Error });
            }

            // 4) Pega o ViewModel retornado pela Application
            var vm = result.Value;

            // 5) Mapeia ViewModel → DTO de saída
            var dto = new PedidoDetalhesDto
            {
                PedidoId = vm.PedidoId,
                ClienteId = vm.ClienteId,
                ValorTotal = vm.ValorTotal,
                Status = vm.Status,
                DataCriacao = vm.DataCriacao,
                Itens = vm.Itens
                    .Select(i => new PedidoItemDetalheDto
                    {
                        ProdutoId = i.ProdutoId,
                        NomeProduto = i.NomeProduto,
                        Quantidade = i.Quantidade,
                        PrecoUnitario = i.PrecoUnitario,
                        Subtotal = i.Subtotal
                    })
                    .ToList()
            };

            // 6) Finalmente, responde ao cliente com 200 OK e o JSON do DTO
            return Ok(dto);
        }
    }
}
