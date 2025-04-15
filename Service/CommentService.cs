using Contracts;
using Service.Contracts;

namespace Service;

internal sealed class CommentService : ICommentService
{
    private readonly IRepositoryManager _repository;
    private readonly ILoggerManager _logger;

    public CommentService(IRepositoryManager repository, ILoggerManager logger)
    {
        _repository = repository;
        _logger = logger;
    }
}