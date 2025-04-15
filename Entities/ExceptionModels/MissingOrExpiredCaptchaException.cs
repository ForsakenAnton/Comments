// Ignore Spelling: Captcha

namespace Entities.ExceptionModels;

public class MissingOrExpiredCaptchaException : BadRequestException
{
    public MissingOrExpiredCaptchaException() 
        : base("Captcha code is missing or expired")
    {
    }
}
