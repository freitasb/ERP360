namespace ERP360.Pedidos.Api.Contracts.Pedidos.CriarPedido
{
    public sealed class CriarPedidoItemDto
    {
        public Guid ProdutoId { get; set; }
        public string NomeProduto { get; set; } = string.Empty;
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
    }
}
