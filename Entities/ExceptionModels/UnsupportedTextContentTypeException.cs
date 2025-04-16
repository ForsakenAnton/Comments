namespace Entities.ExceptionModels;

public class UnsupportedTextContentTypeException : BadRequestException
{
    public UnsupportedTextContentTypeException() 
        : base("Text file should have a '.txt' extension")
    {
    }
}
