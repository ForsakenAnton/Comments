// Ignore Spelling: Captcha

using AutoMapper;
using Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Service.Contracts;
using Shared.Options;

namespace Service;

public sealed class ServiceManager : IServiceManager
{
    private readonly Lazy<IUserService> _userService;
    private readonly Lazy<ICommentService> _commentService;
    private readonly Lazy<IGenerateCaptchaService> _generateCaptchaService;
    private readonly Lazy<IGenerateFileNameService> _generateFileNameService;
    private readonly Lazy<IImageFileService> _imageFileService;
    private readonly Lazy<ITextFileService> _textFileService;
    private readonly Lazy<IHtmlSanitizerService> _htmlSanitizerService;

    public ServiceManager(
        IRepositoryManager repositoryManager, 
        ILoggerManager logger,
        IMapper mapper,
        IOptions<FileStorageOptions> options
        //IHttpContextAccessor httpContextAccessor
        )
    {
        string webRootPath = options.Value.WebRootPath;

        _userService = new Lazy<IUserService>(() => 
            new UserService(repositoryManager, logger, mapper));

        _commentService = new Lazy<ICommentService>(() => 
            new CommentService(repositoryManager, logger, mapper));

        _generateCaptchaService = new Lazy<IGenerateCaptchaService>(() =>
            new GenerateCaptchaService());
            //new GenerateCaptchaService(httpContextAccessor));

        _generateFileNameService = new Lazy<IGenerateFileNameService>(() =>
            new GenerateFileNameService());

        _imageFileService = new Lazy<IImageFileService>(() =>
            new ImageFileService(webRootPath));

        _textFileService = new Lazy<ITextFileService>(() =>
            new TextFileService(webRootPath));

        _htmlSanitizerService = new Lazy<IHtmlSanitizerService>(() =>
            new HtmlSanitizerService());
    }

    public IUserService UserService => _userService.Value;
    public ICommentService CommentService => _commentService.Value;

    public IGenerateCaptchaService GenerateCaptchaService => _generateCaptchaService.Value;
    public IGenerateFileNameService GenerateFileNameService => _generateFileNameService.Value;
    public IImageFileService ImageFileService => _imageFileService.Value;
    public ITextFileService TextFileService => _textFileService.Value;
    public IHtmlSanitizerService HtmlSanitizerService => _htmlSanitizerService.Value;
}
