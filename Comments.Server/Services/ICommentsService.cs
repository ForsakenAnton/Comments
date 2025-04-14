using Comments.Server.Models.Dtos;
using Comments.Server.Models.RequestFeatures;

namespace Comments.Server.Services;

public interface ICommentsService
{
    public Task<(
        IEnumerable<CommentGetDto> comments,
        MetaData MetaData
        )> GetCommentsAsync(CommentParameters commentParameters);
}
