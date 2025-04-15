// Ignore Spelling: Dto Captcha

using AutoMapper;
using Comments.Server.ActionFilters;
using Comments.Server.Models.Dtos;
using Comments.Server.Models.ExceptionModels;
using Comments.Server.Models.RequestFeatures;
using Comments.Server.Services.Contracts;
using Contracts;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Comments.Server.Controllers;

[Route("api/comments")]
[ApiController]
public class CommentsController : ControllerBase
{
    private readonly ICommentsService _commentsService;
    private readonly IGenerateCaptchaService _generateCaptchaService;

    private readonly ILoggerManager _loggerManager;
    public CommentsController(
        ICommentsService commentsService, 
        IGenerateCaptchaService generateCaptchaService,
        ILoggerManager loggerManager)
    {
        _commentsService = commentsService;
        _generateCaptchaService = generateCaptchaService;
        _loggerManager = loggerManager;
    }

    // GET: api/<CommentsController>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CommentGetDto>>> GetComments(
        [FromQuery] CommentParameters commentParameters)
    {
        var (commentDtos, metadata) = await _commentsService
            .GetCommentsAsync(commentParameters);

        Response.Headers.Append(
            "X-Pagination",
            JsonSerializer.Serialize(metadata));

        return Ok(commentDtos);
    }

    [HttpGet(template: "captcha")]
    public async Task<IActionResult> GetCaptcha()
    {
        (string code, byte[] imageBytes) = await _generateCaptchaService.GenerateCaptcha();

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

        CommentGetDto result = await _commentsService.CreateCommentAsync(comment);

        return CreatedAtAction(nameof(CreateComment), new { id = result.Id }, result);
    }
}
