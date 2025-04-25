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
    private readonly ILoggerManager _loggerManager;

    public CommentsController(
        IServiceManager serviceManager,
        ILoggerManager loggerManager)
    {
        _serviceManager = serviceManager;
        _loggerManager = loggerManager;
    }


    // GET: api/comments
    [HttpGet("AllCommentsWithChildren")]
    public async Task<ActionResult<IEnumerable<CommentGetDto>>> GetAllCommentsWithChildren(
        [FromQuery] Shared.RequestFeatures.CommentParameters commentParameters)
    {
        var (commentDtos, metadata) = await _serviceManager.CommentService
            .GetAllCommentsWithChildrenAsync(
                commentParameters: commentParameters,
                trackChanges: true,
                includeUserExpression: (c => c.User!));

        Response.Headers.Append(
            "X-Pagination",
            JsonSerializer.Serialize(metadata));

        return Ok(commentDtos);
    }


    [HttpGet("ParentComments")]
    public async Task<ActionResult<IEnumerable<CommentGetDto>>> GetParentComments(
        [FromQuery] Shared.RequestFeatures.CommentParameters commentParameters)
    {
        var (commentDtos, metadata) = await _serviceManager.CommentService
            .GetParentCommentsAsync(
                commentParameters: commentParameters,
                trackChanges: false,
                includeExpressions: [c => c.User!, c => c.Replies!]);

        Response.Headers.Append(
            "X-Pagination",
            JsonSerializer.Serialize(metadata));

        return Ok(commentDtos);
    }


    [HttpGet("GetChildrenComments/{id:int}")]
    public async Task<ActionResult<IEnumerable<CommentGetDto>>> GetChildrenComments(int id)
    {
        // Why trackChanges is true: comment => user, comment => reply => user, comment => reply => reply (the last for count)
        var commentDtos = await _serviceManager.CommentService
            .GetChildrenCommentsAsync(id, trackChanges: true);

        return Ok(commentDtos);
    }


    // GET: api/comments/captcha
    [HttpGet(template: "captcha/{parentCommentId:int?}")]
    public async Task<IActionResult> GetCaptcha(int? parentCommentId)
    {
        //(string code, byte[] imageBytes, bool isNewCode) =
        //    await _serviceManager.GenerateCaptchaService.GenerateCaptcha();

        //if (isNewCode)
        //{
        //    HttpContext.Session.SetString("CaptchaCode", code);
        //}

        (string code, byte[] imageBytes) =
            await _serviceManager.GenerateCaptchaService.GenerateCaptcha();

        string sessionKey = "CaptchaCode" + parentCommentId?.ToString();
        HttpContext.Session.SetString(sessionKey, code);

        return File(imageBytes, "image/png");
    }


    // POST api/comments
    [HttpPost]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    //[RequestSizeLimit(...)]
    public async Task<IActionResult> CreateComment(
        [FromForm] CommentCreateDto comment)
    {
        string sessionKey = "CaptchaCode" + comment.ParentId?.ToString();
        string? expectedCode = HttpContext.Session.GetString(sessionKey);

        if (string.IsNullOrEmpty(expectedCode))
        {
            throw new MissingOrExpiredCaptchaException();
        }

        if (!string.Equals(comment.Captcha?.Trim(), expectedCode, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidCaptchaException();
        }

        //// Here I remove (and don't remove 0_o, I don't know yet ) the captcha
        HttpContext.Session.Remove(sessionKey);

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


        CommentGetDto result = await _serviceManager.CommentService
            .CreateCommentAsync(comment, imageFileNameForSave, textFileNameForSave);

        return CreatedAtAction(nameof(CreateComment), new { id = result.Id }, result);
    }
}
