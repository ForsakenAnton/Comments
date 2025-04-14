namespace Comments.Server.Models.ErrorModels
{
    public class UnsupportedImageContentTypeException: BadRequestException
    {
        public UnsupportedImageContentTypeException(string error)
            :base(error)
        {
            
        }
    }
}
