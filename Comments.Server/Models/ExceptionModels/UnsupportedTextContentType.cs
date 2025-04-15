namespace Comments.Server.Models.ExceptionModels;

public class UnsupportedTextContentType : BadRequestException
{
    public UnsupportedTextContentType() 
        : base("Text file should have a '.txt' extension")
    {
    }
}
