namespace Comments.Server.Models.ExceptionModels;

public class TooBigFileSizeException : BadRequestException
{
    public TooBigFileSizeException() 
        : base("File size is bigger than allowed.")
    {
    }
}
