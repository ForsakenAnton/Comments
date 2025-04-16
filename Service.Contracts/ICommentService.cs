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
        )> GetCommentsAsync(
            CommentParameters commentParameters,
            bool trackChanges,
            Expression<Func<Comment, User>>? includeUserExpression);

    public Task<CommentGetDto> CreateCommentAsync(
        CommentCreateDto commentDto,
        string? imageFileNameForSave,
        string? textFileNameForSave);
}
