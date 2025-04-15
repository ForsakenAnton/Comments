using Comments.Server.Models.ExceptionModels;
using Comments.Server.Services.Contracts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Comments.Server.Services;

public class ImageFileService : IImageFileService
{
    private readonly IWebHostEnvironment _environment;
    private readonly string[] _allowedImageTypes = new[] 
    {
        "image/jpeg",
        "image/png",
        "image/gif",
    };

    public ImageFileService(IWebHostEnvironment environment)
    {
        _environment = environment;
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
        string savePath = Path.Combine(_environment.WebRootPath, relativePath);

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
