using AutoMapper;
using Comments.Server.Data;
using Comments.Server.Data.Entities;
using Comments.Server.Models.Dtos;
using Comments.Server.Models.RequestFeatures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Comments.Server.Controllers;

[Route("api/comments")]
[ApiController]
public class CommentsController : ControllerBase
{
    private readonly ILogger<CommentsController> _logger;
    private readonly IMapper _mapper;
    private readonly CommentsDbContext _commentsDbContext;

    public CommentsController(
        ILogger<CommentsController> logger,
        IMapper mapper,
        CommentsDbContext commentsDbContext)
    {
        _logger = logger;
        _mapper = mapper;
        _commentsDbContext = commentsDbContext;
    }


    // GET: api/<CommentsController>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CommentGetDto>>> GetComments(
        [FromQuery] CommentParameters commentParameters)
    {
        IQueryable<Comment> comments = _commentsDbContext.Comments
            .Include(c => c.User);

        // sorting
        comments = commentParameters.OrderBy switch
        {
            "date desc" => comments.OrderByDescending(c => c.CreationDate),
            "date" or "date asc" => comments.OrderBy(c => c.CreationDate), // !!!
            "user_name" or "user_name asc" => comments.OrderBy(c => c.User!.UserName),
            "user_name desc" => comments.OrderByDescending(c => c.User!.UserName),
            "user_email" or "user_email asc" => comments.OrderBy(c => c.User!.Email),
            "user_email desc" => comments.OrderByDescending(c => c.User!.Email),
            _ => comments.OrderByDescending(c => c.CreationDate)
        };

        // pagination
        comments = comments
            .Skip((commentParameters.PageNumber - 1) * commentParameters.PageSize)
            .Take(commentParameters.PageSize);

        var pagedCommentsWithMetaData = new PagedList<Comment>(
            items: await comments.ToListAsync(),
            totalCount: await _commentsDbContext.Comments.CountAsync(),
            currentPage: commentParameters.PageNumber,
            pageSize: commentParameters.PageSize);

        var commentDtos = _mapper
            .Map<List<CommentGetDto>>(pagedCommentsWithMetaData);

        Response.Headers.Append(
            "X-Pagination",
            JsonSerializer.Serialize(pagedCommentsWithMetaData.MetaData));

        return Ok(commentDtos);
    }
}
