// Ignore Spelling: Captcha

namespace Comments.Server.Models.CaptchaModels;

public class CaptchaValidationResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
}
