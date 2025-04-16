// Ignore Spelling: Captcha

using AutoMapper;
using Contracts;
using Service.Contracts;

namespace Service;

public sealed class ServiceManager : IServiceManager
{
    private readonly Lazy<IUserService> _userService;
    private readonly Lazy<ICommentService> _commentService;
    private readonly Lazy<IGenerateCaptchaService> _generateCaptchaService;

    public ServiceManager(
        IRepositoryManager repositoryManager, 
        ILoggerManager logger,
        IMapper mapper)
    {
        _userService = new Lazy<IUserService>(() => 
            new UserService(repositoryManager, logger, mapper));

        _commentService = new Lazy<ICommentService>(() => 
            new CommentService(repositoryManager, logger, mapper));

        _generateCaptchaService = new Lazy<IGenerateCaptchaService>(() =>
            new GenerateCaptchaService());
    }

    public IUserService UserService => _userService.Value;
    public ICommentService CommentService => _commentService.Value;

    public IGenerateCaptchaService GenerateCaptchaService => _generateCaptchaService.Value;
}
