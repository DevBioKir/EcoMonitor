namespace EcoMonitor.Contracts.Models;

public sealed record PagedResult<T>
{
    public List<T> Items { get; private set; } = new();
    public int TotalCount { get; private set; }
    public int Page { get; private set; }
    public int PageSize { get; private set; }

    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;

    public PagedResult(List<T> items, int totalCount, int page, int pageSize)
    {
        Items = items ?? new List<T>();
        TotalCount = totalCount;
        Page = page;
        PageSize = pageSize;
    }
}