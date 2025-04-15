namespace Comments.Server.Models.ExceptionModels
{
    public class BadRequestException : InvalidOperationException
    {
        public BadRequestException(string error) 
            : base(error)
        {
            
        }
    }
}
