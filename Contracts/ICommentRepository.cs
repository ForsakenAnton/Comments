using Entities.Models;
using Shared.RequestFeatures;
using System.Linq.Expressions;

namespace Contracts;

public interface ICommentRepository
{
    Task<PagedList<Comment>> GetCommentsAsync(
        CommentParameters commentParameters,
        bool trackChanges,
        Expression<Func<Comment, User>>? includeUserExpression);

    Task<Comment?> GetCommentByIdAsync(int id, bool trackChanges);
    public Task CreateComment(Comment comment);
}
