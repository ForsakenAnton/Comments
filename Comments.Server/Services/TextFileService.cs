using Comments.Server.Models.ErrorModels;
using Comments.Server.Services.Contracts;
using Microsoft.CodeAnalysis.Text;

namespace Comments.Server.Services;

public class TextFileService : ITextFileService
{
    private readonly IWebHostEnvironment _environment;

    private readonly string _allowedType = "text/plain";
    private readonly long _maxSize = 100 * 1024;

    public TextFileService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task SaveFileAsync(IFormFile textFile, string fileNameForSave)
    {
        if (textFile is null)
        {
            return;
        }

        CheckIsValidSize(textFile.Length);
        CheckIsValidType(textFile.ContentType);

        string relativePath = Path.Combine("textFiles", fileNameForSave);
        string savePath = Path.Combine(_environment.WebRootPath, relativePath);

        using FileStream outputStream = new FileStream(savePath, FileMode.Create);
        await textFile.CopyToAsync(outputStream);
    }

    private void CheckIsValidSize(long size)
    {
        if (size > _maxSize)
        {
            throw new TooBigFileSizeException("File size is bigger than 100KB.");
        }
    }

    private void CheckIsValidType(string contentType)
    {
        if (contentType != _allowedType)
        {
            throw new TooBigFileSizeException("File size is bigger than 100KB.");
        }
    }
}
