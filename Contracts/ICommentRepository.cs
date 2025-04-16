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

    public Task CreateComment(Comment comment);
}
