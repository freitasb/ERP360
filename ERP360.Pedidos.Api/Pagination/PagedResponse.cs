namespace ERP360.Pedidos.Api.Pagination
{
    public sealed class PagedResponse<T>
    {
        public required IReadOnlyList<T> Items { get; init; }
        public required int Page { get; init; }
        public required int PageSize { get; init; }
        public required int TotalItems { get; init; }
        public required int TotalPages { get; init; }


        public static PagedResponse<T> Create(IReadOnlyList<T> items, int page, int pageSize, int totalItems)
        {
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            return new PagedResponse<T>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            };
        }
    }
}
