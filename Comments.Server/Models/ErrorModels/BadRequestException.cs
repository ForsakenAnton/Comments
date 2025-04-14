namespace Comments.Server.Models.ErrorModels
{
    public class BadRequestException : InvalidOperationException
    {
        public BadRequestException(string error) 
            : base(error)
        {
            
        }
    }
}
