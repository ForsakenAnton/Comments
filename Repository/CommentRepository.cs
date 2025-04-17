using AngleSharp.Dom;
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

    public async Task<PagedList<Comment>> GetAllCommentsWithChildrenAsync(
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


    public async Task<PagedList<Comment>> GetParentCommentsAsync(
        CommentParameters commentParameters,
        bool trackChanges,
        params Expression<Func<Comment, object>>[] includeExpressions)
    {
        IQueryable<Comment> rootComments = base
            .FindAllWithIncludes(trackChanges, includeExpressions)
            .Where(c => c.ParentId == null);

        // Sorting
        rootComments = CommentSortingHelper.ApplySorting(
            rootComments,
            commentParameters.OrderBy);

        // Pagination
        rootComments = rootComments
            .Skip((commentParameters.PageNumber - 1) * commentParameters.PageSize)
            .Take(commentParameters.PageSize);


        var pagedCommentsWithMetaData = new PagedList<Comment>(
            items: await rootComments.ToListAsync(),
            totalCount: await base.RepositoryContext.Comments.CountAsync(c => c.ParentId == null),
            currentPage: commentParameters.PageNumber,
            pageSize: commentParameters.PageSize);

        return pagedCommentsWithMetaData;
    }


    public async Task<Comment?> GetCommentByIdAsync(
        int id, 
        bool trackChanges)
    {
        var comment = await base
            .FindByConditionWithIncludes(c => c.Id == id, trackChanges)
            .FirstOrDefaultAsync();

        return comment;
    }

    public async Task<Comment?> GetCommentByIdWithNestedIncludes(int id, bool trackChanges)
    {
        var query = base.FindAllWithNestedIncludes(
            trackChanges,
            q => q.Include(c => c.User),
            q => q.Include(c => c.Replies),
            q => q.Include(c => c.Replies)!.ThenInclude(r => r.Replies)); // here I need only replies' count

        return await query.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task CreateComment(Comment comment)
    {
        await base.CreateAsync(comment);
    }
}
