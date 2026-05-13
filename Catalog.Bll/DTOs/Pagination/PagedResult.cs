namespace Catalog.Bll.DTOs.Pagination
{
    public record PagedResult<T>(IEnumerable<T> Items, int TotalCount, int Page, int PageSize);
}
