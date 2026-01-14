using ERP360.Estoque.Api.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace ERP360.Estoque.Api.Controllers
{
    [ApiController]
    [Route("api/v1/estoque/reservas")]
    public sealed class ReservasController : ControllerBase
    {
        private readonly IReservaRepository _repo;

        public ReservasController(IReservaRepository repo) => _repo = repo;

        [HttpGet("por-pedido/{pedidoId:guid}")]
        public async Task<IActionResult> ObterPorPedido(Guid pedidoId, CancellationToken ct)
        {
            var reserva = await _repo.GetByPedidoIdAsync(pedidoId, ct);
            if (reserva is null) return NotFound();

            return Ok(reserva);
        }
    }
}
