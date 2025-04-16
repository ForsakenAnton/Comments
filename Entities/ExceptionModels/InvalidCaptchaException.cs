// Ignore Spelling: Captcha

namespace Entities.ExceptionModels;

public sealed class InvalidCaptchaException : BadRequestException
{
    public InvalidCaptchaException() 
        : base("Captcha is invalid")
    {
    }
}
