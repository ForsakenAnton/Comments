namespace Entities.ExceptionModels;

public sealed class UnsupportedTextContentTypeException : BadRequestException
{
    public UnsupportedTextContentTypeException() 
        : base("Text file should have a '.txt' extension")
    {
    }
}
