namespace Comments.Server.Models.RequestFeatures;

public abstract class RequestParameters
{
    /// <summary>
    /// Сортування. Можливі значення:
    /// - date
    /// - date desc
    /// - user_name
    /// - user_name desc
    /// - user_email
    /// - user_email desc
    /// </summary>
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
