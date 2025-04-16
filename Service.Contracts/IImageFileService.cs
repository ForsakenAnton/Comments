using Microsoft.AspNetCore.Http;

namespace Service.Contracts;

public interface IImageFileService
{
    Task ResizeAndSaveImageAsync(
        IFormFile imageFile,
        string fileNameForSave, 
        int maxWidth = 320,
        int maxHeight = 240);
}
