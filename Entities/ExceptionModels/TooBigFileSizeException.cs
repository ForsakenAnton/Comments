namespace Entities.ExceptionModels;

public sealed class TooBigFileSizeException : BadRequestException
{
    public TooBigFileSizeException() 
        : base("File size is bigger than allowed.")
    {
    }
}
