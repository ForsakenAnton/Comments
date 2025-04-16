// Ignore Spelling: Dto Captcha

using Comments.Server.ActionFilters;
using Contracts;
using Entities.ExceptionModels;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.Dtos;
using System.Text.Json;

namespace Comments.Server.Controllers;

[Route("api/comments")]
[ApiController]
public class CommentsController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    private readonly ICommentService _commentsService;

    private readonly ILoggerManager _loggerManager;
    public CommentsController(
        IServiceManager serviceManager,
        ICommentService commentsService,
        ILoggerManager loggerManager)
    {
        _serviceManager = serviceManager;

        _commentsService = commentsService;
        _loggerManager = loggerManager;
    }

    // GET: api/<CommentsController>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CommentGetDto>>> GetComments(
        [FromQuery] Shared.RequestFeatures.CommentParameters commentParameters)
    {
        var (commentDtos, metadata) = await _serviceManager.CommentService
            .GetCommentsAsync(
                commentParameters: commentParameters,
                trackChanges: true,
                includeUserExpression: (c => c.User!));

        //var (commentDtos, metadata) = await _commentsService
        //    .GetCommentsAsync(commentParameters);

        Response.Headers.Append(
            "X-Pagination",
            JsonSerializer.Serialize(metadata));

        return Ok(commentDtos);
    }

    [HttpGet(template: "captcha")]
    public async Task<IActionResult> GetCaptcha()
    {
        (string code, byte[] imageBytes) = await _serviceManager.GenerateCaptchaService.GenerateCaptcha();

        HttpContext.Session.SetString("CaptchaCode", code);

        return File(imageBytes, "image/png");
    }

    [HttpPost]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    //[RequestSizeLimit(...)]
    public async Task<IActionResult> CreateComment(
        [FromForm] CommentCreateDto comment)
    {
        string? expectedCode = HttpContext.Session.GetString("CaptchaCode");

        if (string.IsNullOrEmpty(expectedCode))
        {
            throw new MissingOrExpiredCaptchaException();
        }

        if (!string.Equals(comment.Captcha?.Trim(), expectedCode, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidCaptchaException();
        }

        // Here I remove the captcha
        HttpContext.Session.Remove("CaptchaCode");

        // Sanitize text with existing html
        comment.Text = await _serviceManager.HtmlSanitizerService.SanitizeTextAsync(comment.Text);

        // image and text...
        string? imageFileNameForSave = null;
        string? textFileNameForSave = null;

        if (comment.ImageFile is not null)
        {
            imageFileNameForSave = await _serviceManager.GenerateFileNameService
                .GenerateFileNameAsync(comment.ImageFile);

            await _serviceManager.ImageFileService
                .ResizeAndSaveImageAsync(comment.ImageFile, imageFileNameForSave);
        }

        if (comment.TextFile is not null)
        {
            textFileNameForSave = await _serviceManager.GenerateFileNameService
                .GenerateFileNameAsync(comment.TextFile);

            await _serviceManager.TextFileService
                .SaveFileAsync(comment.TextFile, textFileNameForSave);
        }


        CommentGetDto result = await _commentsService
            .CreateCommentAsync(comment, imageFileNameForSave, textFileNameForSave);

        return CreatedAtAction(nameof(CreateComment), new { id = result.Id }, result);
    }
}
