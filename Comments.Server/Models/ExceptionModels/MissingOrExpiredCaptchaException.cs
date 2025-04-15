// Ignore Spelling: Captcha

namespace Comments.Server.Models.ExceptionModels;

public class MissingOrExpiredCaptchaException : BadRequestException
{
    public MissingOrExpiredCaptchaException() 
        : base("Captcha code is missing or expired")
    {
    }
}
