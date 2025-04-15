// Ignore Spelling: Captcha

namespace Comments.Server.Services.Contracts;

public interface IGenerateCaptchaService
{
    Task<(string code, byte[] imageBytes)> GenerateCaptcha();
}
