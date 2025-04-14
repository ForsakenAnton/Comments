using Comments.Server.Models.Dtos;
using Comments.Server.Models.RequestFeatures;

namespace Comments.Server.Services.Contracts;

public interface ICommentsService
{
    Task<(
        IEnumerable<CommentGetDto> comments,
        MetaData MetaData
        )> GetCommentsAsync(CommentParameters commentParameters);
}
