using Microsoft.AspNetCore.Http;
using Service.Contracts;

namespace Service;

internal sealed class GenerateFileNameService : IGenerateFileNameService
{
    public Task<string> GenerateFileNameAsync(IFormFile formFile)
    {
        string fileName = $"{Guid.NewGuid()}{formFile.FileName}";

        return Task.FromResult(fileName);
    }
}
