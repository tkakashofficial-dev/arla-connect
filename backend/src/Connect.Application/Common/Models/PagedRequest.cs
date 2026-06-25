namespace Connect.Application.Common.Models;

/// <summary>
/// Base for paged queries. Page/PageSize are clamped so a client can never ask
/// for page 0 or pull 10,000 rows at once (a common scalability footgun).
/// </summary>
public abstract class PagedRequest
{
    private const int MaxPageSize = 100;
    private int _page = 1;
    private int _pageSize = 20;

    public int Page
    {
        get => _page;
        set => _page = value < 1 ? 1 : value;
    }

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value is < 1 or > MaxPageSize ? 20 : value;
    }

    public int Skip => (Page - 1) * PageSize;
}
