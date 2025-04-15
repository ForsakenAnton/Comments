namespace Comments.Server.Models.ExceptionModels;

public class UnsupportedImageContentTypeException: BadRequestException
{
    public UnsupportedImageContentTypeException()
        :base("Only JPEG, PNG and GIF images are allowed.")
    {
        
    }
}
