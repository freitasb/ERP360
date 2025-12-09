namespace ERP360.Pedidos.Api.Contracts.Pedidos
{
    public sealed class CriarPedidoDto
    {
        public Guid ClienteId { get; set; }
        public List<CriarPedidoItemDto> Itens { get; set; } = new();
    }
}
