namespace Comments.Server.Services.Contracts;

public interface IImageService
{
    Task ResizeAndSaveImageAsync(
        IFormFile imageFile,
        string fileNameForSave, 
        int maxWidth = 320,
        int maxHeight = 240);
}
