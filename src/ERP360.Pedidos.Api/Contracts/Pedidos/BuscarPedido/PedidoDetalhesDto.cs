namespace ERP360.Pedidos.Api.Contracts.Pedidos.BuscarPedido
{
    public sealed class PedidoDetalhesDto
    {
        public Guid PedidoId { get; set; }
        public Guid ClienteId { get; set; }
        public decimal ValorTotal { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTimeOffset DataCriacao { get; set; }
        public List<PedidoItemDetalheDto> Itens { get; set; } = new();
    }
}
