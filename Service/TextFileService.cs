
using Entities.ExceptionModels;
using Microsoft.AspNetCore.Http;
using Service.Contracts;

namespace Service;

internal sealed class TextFileService : ITextFileService
{
    private readonly string _allowedType = "text/plain";
    private readonly long _maxSize = 100 * 1024;

    private readonly string _webRootPath;
    public TextFileService(string webRootPath)
    {
        _webRootPath = webRootPath;
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
        string savePath = Path.Combine(_webRootPath, relativePath);

        using FileStream outputStream = new FileStream(savePath, FileMode.Create);
        await textFile.CopyToAsync(outputStream);
    }

    private void CheckIsValidSize(long size)
    {
        if (size > _maxSize)
        {
            throw new TooBigFileSizeException();
        }
    }

    private void CheckIsValidType(string contentType)
    {
        if (contentType != _allowedType)
        {
            throw new UnsupportedTextContentTypeException();
        }
    }
}
