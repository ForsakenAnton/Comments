namespace Entities.ExceptionModels;

public abstract class BadRequestException : InvalidOperationException
{
    public BadRequestException(string error) 
        : base(error)
    {
        
    }
}
