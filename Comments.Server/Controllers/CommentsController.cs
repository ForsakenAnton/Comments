// Ignore Spelling: Dto

using AutoMapper;
using Comments.Server.ActionFilters;
using Comments.Server.Models.Dtos;
using Comments.Server.Models.RequestFeatures;
using Comments.Server.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Comments.Server.Controllers;

[Route("api/comments")]
[ApiController]
public class CommentsController : ControllerBase
{
    private readonly ICommentsService _commentsService;

    public CommentsController(ICommentsService commentsService)
    {
        _commentsService = commentsService;
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


    [HttpPost]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    //[RequestSizeLimit(...)]
    public async Task<IActionResult> CreateComment(CommentCreateDto comment)
    {
        var result = await _commentsService.CreateCommentAsync(comment);
        return CreatedAtAction(nameof(CreateComment), new { id = result.Id }, result);
    }
}
