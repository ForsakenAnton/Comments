// Ignore Spelling: Captcha

namespace Service.Contracts;

public interface IGenerateCaptchaService
{
    //Task<(string code, byte[] imageBytes, bool isNewCode)> GenerateCaptcha();
    Task<(string code, byte[] imageBytes)> GenerateCaptcha();
}
