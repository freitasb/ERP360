namespace ERP360.Pedidos.Api.Pagination
{
    public record PaginationParams(int Page = 1, int PageSize = 20)
    {
        public PaginationParams Validate()
        {
            var p = Math.Max(1, Page);
            var s = Math.Clamp(PageSize, 1, 100); // Proteção de performance
            return new PaginationParams(p, s);
        }
    }
}
