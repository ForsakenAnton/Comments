namespace Comments.Server.Models.ErrorModels
{
    public class UnsupportedTextContentType : BadRequestException
    {
        public UnsupportedTextContentType(string error) 
            : base(error)
        {
        }
    }
}
