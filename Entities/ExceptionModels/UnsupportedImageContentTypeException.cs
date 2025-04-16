namespace Entities.ExceptionModels;

public sealed class UnsupportedImageContentTypeException: BadRequestException
{
    public UnsupportedImageContentTypeException()
        :base("Only JPEG, PNG and GIF images are allowed.")
    {
        
    }
}
