using Contracts;
using Service.Contracts;

namespace Service;

public sealed class ServiceManager : IServiceManager
{
    private readonly Lazy<IUserService> _userService;
    private readonly Lazy<ICommentService> _commentService;

    public ServiceManager(IRepositoryManager repositoryManager, ILoggerManager logger)
    {
        _userService = new Lazy<IUserService>(() => 
            new UserService(repositoryManager, logger));

        _commentService = new Lazy<ICommentService>(() => 
            new CommentService(repositoryManager, logger));
    }

    public IUserService UserService => _userService.Value;
    public ICommentService CommentService => _commentService.Value;
}
