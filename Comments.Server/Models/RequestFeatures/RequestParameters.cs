namespace Comments.Server.Models.RequestFeatures;

public abstract class RequestParameters
{
    public string? OrderBy { get; set; }

    const int maxPageSize = 25;
    public int PageNumber { get; set; } = 1;

    private int _pageSize = 5;

    public int PageSize
    {
        get
        {
            return _pageSize;
        }
        set
        {
            _pageSize = (value > maxPageSize) ? maxPageSize : value;
        }
    }
}
