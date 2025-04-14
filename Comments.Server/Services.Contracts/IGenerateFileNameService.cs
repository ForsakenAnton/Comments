namespace Comments.Server.Services.Contracts;

public interface IGenerateFileNameService
{
    Task<string> GenerateFileName(IFormFile formFile);
}
