namespace Entities.ExceptionModels;

public class BadRequestException : InvalidOperationException
{
    public BadRequestException(string error) 
        : base(error)
    {
        
    }
}
