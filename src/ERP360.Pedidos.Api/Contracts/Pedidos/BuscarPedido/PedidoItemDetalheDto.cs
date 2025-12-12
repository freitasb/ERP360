namespace ERP360.Pedidos.Api.Contracts.Pedidos.BuscarPedido
{
    public sealed class PedidoItemDetalheDto
    {
        public Guid ProdutoId { get; set; }
        public string NomeProduto { get; set; } = string.Empty;
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }
}
