using AutoMapper;
using Comments.Server.Data;
using Comments.Server.Data.Entities;
using Comments.Server.Models.Dtos;
using Comments.Server.Models.RequestFeatures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

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
        IQueryable<Comment> rootComments = _commentsDbContext.Comments
            .Include(c => c.User)
            .Where(c => c.ParentId == null);

        // Sorting
        rootComments = CommentSortingHelper.ApplySorting(
            rootComments, 
            commentParameters.OrderBy);


        // Pagination
        rootComments = rootComments
            .Skip((commentParameters.PageNumber - 1) * commentParameters.PageSize)
            .Take(commentParameters.PageSize);

        // Here I explicitly load related comments (children comments)
        // and users of comments, because the users loaded before 
        // have relation only to root comments
        await _commentsDbContext.Comments.LoadAsync();
        await _commentsDbContext.Users.LoadAsync();

        
        var pagedCommentsWithMetaData = new PagedList<Comment>(
            items: await rootComments.ToListAsync(),
            totalCount: await _commentsDbContext.Comments.CountAsync(c => c.ParentId == null),
            currentPage: commentParameters.PageNumber,
            pageSize: commentParameters.PageSize);

        // Here I map to CommentGetDto and have the rootComments
        // with three of children, and don't have circular references
        var commentDtos = _mapper
            .Map<List<CommentGetDto>>(pagedCommentsWithMetaData);

        Response.Headers.Append(
            "X-Pagination",
            JsonSerializer.Serialize(pagedCommentsWithMetaData.MetaData));

        return Ok(commentDtos);
    }
}
