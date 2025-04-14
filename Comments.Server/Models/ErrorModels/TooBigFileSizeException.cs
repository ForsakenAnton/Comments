namespace Comments.Server.Models.ErrorModels;

public class TooBigFileSizeException : BadRequestException
{
    public TooBigFileSizeException(string error) 
        : base(error)
    {
    }
}
