using AutoMapper;
using Contracts;
using Entities.Models;
using Service.Contracts;
using Shared.Dtos;
using Shared.RequestFeatures;
using System.Linq.Expressions;

namespace Service;

internal sealed class CommentService : ICommentService
{
    private readonly IRepositoryManager _repository;
    private readonly ILoggerManager _logger;
    private readonly IMapper _mapper;

    public CommentService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
    {
        _repository = repository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<(
        IEnumerable<CommentGetDto> comments, 
        MetaData MetaData)> GetCommentsAsync(
            CommentParameters commentParameters, 
            bool trackChanges, 
            Expression<Func<Comment, User>>? includeUserExpression)
    {
        var pagedCommentsWithMetaData = await _repository.Comment
            .GetCommentsAsync(commentParameters, trackChanges, includeUserExpression);

        var commentDtos = _mapper
            .Map<List<CommentGetDto>>(pagedCommentsWithMetaData);

        var metadata = pagedCommentsWithMetaData.MetaData;

        return (commentDtos, metadata);
    }
}