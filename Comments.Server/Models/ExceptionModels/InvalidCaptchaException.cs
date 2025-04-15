// Ignore Spelling: Captcha

namespace Comments.Server.Models.ExceptionModels;

public class InvalidCaptchaException : BadRequestException
{
    public InvalidCaptchaException() 
        : base("Captcha is invalid")
    {
    }
}
