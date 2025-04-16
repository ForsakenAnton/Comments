namespace Service.Contracts;

public interface IServiceManager
{
    IUserService UserService { get; }
    ICommentService CommentService { get; }
    IGenerateCaptchaService GenerateCaptchaService { get; }
    IGenerateFileNameService GenerateFileNameService { get; }
    IImageFileService ImageFileService { get; }
    ITextFileService TextFileService { get; }
    IHtmlSanitizerService HtmlSanitizerService { get; }
}
