using Entities.Models;

namespace Shared.RequestFeatures;

public static class CommentSortingHelper
{
    public static IQueryable<Comment> ApplySorting(
        IQueryable<Comment> query,
        string? orderBy)
    {
        return orderBy?.ToLower() switch
        {
            "date desc" => query.OrderByDescending(c => c.CreationDate),
            "date" or "date asc" => query.OrderBy(c => c.CreationDate),
            "user_name" or "user_name asc" => query.OrderBy(c => c.User!.UserName),
            "user_name desc" => query.OrderByDescending(c => c.User!.UserName),
            "user_email" or "user_email asc" => query.OrderBy(c => c.User!.Email),
            "user_email desc" => query.OrderByDescending(c => c.User!.Email),
            _ => query.OrderByDescending(c => c.CreationDate)
        };
    }
}

