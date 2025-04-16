using Microsoft.AspNetCore.Http;

namespace Service.Contracts;

public interface IGenerateFileNameService
{
    Task<string> GenerateFileNameAsync(IFormFile formFile);
}
