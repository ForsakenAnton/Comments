namespace Shared.RequestFeatures;

public class PagedList<T> : List<T>
{
    public MetaData MetaData { get; set; }

    public PagedList(List<T> items, int totalCount, int currentPage, int pageSize)
    {
        AddRange(items);

        MetaData = new MetaData
        {
            CurrentPage = currentPage,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
            PageSize = pageSize,
            TotalCount = totalCount,
        };
    }

    // Now it's not needed
    //public static PagedList<T> ToPagedList(
    //    IEnumerable<T> source,
    //    int pageNumber, 
    //    int pageSize)
    //{
    //    int count = source.Count();

    //    var items = source
    //        .Skip((pageNumber - 1) * pageSize)
    //        .Take(pageSize)
    //        .ToList();

    //    return new PagedList<T>(items, count, pageNumber, pageSize);
    //}
}