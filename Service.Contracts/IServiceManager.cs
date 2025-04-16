namespace Service.Contracts;

public interface IServiceManager
{
    IUserService UserService { get; }
    ICommentService CommentService { get; }
    IGenerateCaptchaService GenerateCaptchaService { get; }
}
