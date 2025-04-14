using Comments.Server.Services.Contracts;

namespace Comments.Server.Services;

public class GenerateFileNameService : IGenerateFileNameService
{
    public Task<string> GenerateFileName(IFormFile formFile)
    {
        string fileName = $"{Guid.NewGuid()}{formFile.FileName}";

        return Task.FromResult(fileName);
    }
}
