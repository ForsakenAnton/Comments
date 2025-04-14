namespace Comments.Server.Services.Contracts;

public interface ITextFileService
{
    Task SaveFileAsync(IFormFile file, string fileNameForSave);
}
