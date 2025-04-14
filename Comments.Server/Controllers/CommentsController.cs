using AutoMapper;
using Comments.Server.Data;
using Comments.Server.Data.Entities;
using Comments.Server.Models.Dtos;
using Comments.Server.Models.RequestFeatures;
using Comments.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
}
