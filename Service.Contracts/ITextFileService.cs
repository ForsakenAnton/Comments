using Microsoft.AspNetCore.Http;

namespace Service.Contracts;

public interface ITextFileService
{
    Task SaveFileAsync(IFormFile file, string fileNameForSave);
}
