// Ignore Spelling: Dto

using Entities.Models;
using Shared.Dtos;
using Shared.RequestFeatures;
using System.Linq.Expressions;

namespace Service.Contracts;

public interface ICommentService
{
    public Task<(
        IEnumerable<CommentGetDto> comments,
        MetaData MetaData
        )> GetAllCommentsWithChildrenAsync(
            CommentParameters commentParameters,
            bool trackChanges,
            Expression<Func<Comment, User>>? includeUserExpression);

    Task<(
        IEnumerable<CommentGetDto> comments,
        MetaData MetaData)> GetParentCommentsAsync(
            CommentParameters commentParameters,
            bool trackChanges,
            params Expression<Func<Comment, object>>[] includeExpressions);

    Task<IEnumerable<CommentGetDto>> GetChildrenCommentsAsync(
        int id, 
        bool trackChanges);

    public Task<CommentGetDto> CreateCommentAsync(
        CommentCreateDto commentDto,
        string? imageFileNameForSave,
        string? textFileNameForSave);

}
