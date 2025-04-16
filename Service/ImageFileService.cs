
using Entities.ExceptionModels;
using Microsoft.AspNetCore.Http;
using Service.Contracts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Service;

internal sealed class ImageFileService : IImageFileService
{
    private readonly string[] _allowedImageTypes = new[] 
    {
        "image/jpeg",
        "image/png",
        "image/gif",
    };

    private readonly string _webRootPath;
    public ImageFileService(string webRootPath)
    {
        _webRootPath = webRootPath;
    }

    public async Task ResizeAndSaveImageAsync(
        IFormFile imageFile, 
        string fileNameForSave, 
        int maxWidth, 
        int maxHeight)
    {
        CheckIsValidImageType(imageFile.ContentType);

        using Stream inputStream = imageFile.OpenReadStream();
        using Image image = await Image.LoadAsync(inputStream);

        if (image.Width > maxWidth || image.Height > maxHeight)
        {
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Max,
                Size = new Size(maxWidth, maxHeight)
            }));
        }

        string relativePath = Path.Combine("images", fileNameForSave);
        string savePath = Path.Combine(_webRootPath, relativePath);

        using FileStream outputStream = new FileStream(savePath, FileMode.Create);

        switch (imageFile.ContentType.ToLower())
        {
            case "image/jpeg":
            case "image/jpg":
                await image.SaveAsJpegAsync(outputStream);
                break;
            case "image/png":
                await image.SaveAsPngAsync(outputStream);
                break;
            case "image/gif":
                await image.SaveAsGifAsync(outputStream);
                break;
        }
    }

    private void CheckIsValidImageType(string contentType)
    {
        if (!_allowedImageTypes.Contains(contentType.ToLower()))
        {
            throw new UnsupportedImageContentTypeException();
        }
    }
}
