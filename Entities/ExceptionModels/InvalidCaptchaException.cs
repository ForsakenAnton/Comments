// Ignore Spelling: Captcha

namespace Entities.ExceptionModels;

public class InvalidCaptchaException : BadRequestException
{
    public InvalidCaptchaException() 
        : base("Captcha is invalid")
    {
    }
}
