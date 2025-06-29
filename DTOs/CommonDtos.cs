namespace netapi_template.DTOs;

public record ApiResponse<T>(
    bool Success,
    string Message,
    T? Data = default,
    IEnumerable<string>? Errors = null
);

public record PagedResult<T>(
    IEnumerable<T> Data,
    int PageNumber,
    int PageSize,
    int TotalPages,
    int TotalRecords,
    bool HasNext,
    bool HasPrevious
)
{
    public static PagedResult<T> Create(IEnumerable<T> data, int pageNumber, int pageSize, int totalRecords)
    {
        var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
        return new PagedResult<T>(
            data,
            pageNumber,
            pageSize,
            totalPages,
            totalRecords,
            pageNumber < totalPages,
            pageNumber > 1
        );
    }
}

public record PaginationQuery(
    int Page = 1,
    int PageSize = 10,
    string? Search = null,
    string? SortBy = null,
    bool SortDescending = false
)
{
    public int Skip => (Page - 1) * PageSize;
    public int Take => PageSize;
}
