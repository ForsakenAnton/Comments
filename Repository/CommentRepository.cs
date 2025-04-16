using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Shared.RequestFeatures;
using System.Linq.Expressions;

namespace Repository;

public class CommentRepository : RepositoryBase<Comment>, ICommentRepository
{
    public CommentRepository(RepositoryContext repositoryContext) 
        : base(repositoryContext)
    {
    }

    public async Task<PagedList<Comment>> GetCommentsAsync(
        CommentParameters commentParameters, 
        bool trackChanges, 
        Expression<Func<Comment, User>>? includeUserExpression)
    {
        IQueryable<Comment> rootComments = base
            .FindAll(trackChanges)
            .Where(c => c.ParentId == null);

        if (includeUserExpression is not null)
        {
            rootComments = rootComments.Include(includeUserExpression);
        }

        // Sorting
        rootComments = CommentSortingHelper.ApplySorting(
            rootComments,
            commentParameters.OrderBy);

        // Pagination
        rootComments = rootComments
            .Skip((commentParameters.PageNumber - 1) * commentParameters.PageSize)
            .Take(commentParameters.PageSize);

        // Here I explicitly load related comments (children comments)
        // and users of comments, because the users loaded before 
        // have relation only to root comments
        await base.RepositoryContext.Comments.LoadAsync();
        await base.RepositoryContext.Users.LoadAsync();

        var pagedCommentsWithMetaData = new PagedList<Comment>(
            items: await rootComments.ToListAsync(),
            totalCount: await base.RepositoryContext.Comments.CountAsync(c => c.ParentId == null),
            currentPage: commentParameters.PageNumber,
            pageSize: commentParameters.PageSize);

        return pagedCommentsWithMetaData;
    }

    public async Task CreateComment(Comment comment)
    {
        await base.CreateAsync(comment);
    }

}
