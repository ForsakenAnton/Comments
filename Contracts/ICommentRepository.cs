using Entities.Models;
using Shared.RequestFeatures;
using System.Linq.Expressions;

namespace Contracts;

public interface ICommentRepository
{
    Task<PagedList<Comment>> GetAllCommentsWithChildrenAsync(
        CommentParameters commentParameters,
        bool trackChanges,
        Expression<Func<Comment, User>>? includeUserExpression);

    Task<PagedList<Comment>> GetParentCommentsAsync(
        CommentParameters commentParameters, 
        bool trackChanges, 
        params Expression<Func<Comment, object>>[] includeExpressions);


    Task<Comment?> GetCommentByIdAsync(
        int id, 
        bool trackChanges);

    Task<IEnumerable<Comment>> GetCommentsWithNestedIncludesAsync(int id, bool trackChanges);

    public Task CreateComment(Comment comment);
}
