using AutoMapper;
using Comments.Server.Controllers;
using Comments.Server.Data;
using Comments.Server.Data.Entities;
using Comments.Server.Models.Dtos;
using Comments.Server.Models.RequestFeatures;
using Comments.Server.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Comments.Server.Services;

public class CommentsService : ICommentsService
{
    private readonly ILogger<CommentsController> _logger;
    private readonly IMapper _mapper;
    private readonly CommentsDbContext _commentsDbContext;

    public CommentsService(
        ILogger<CommentsController> logger,
        IMapper mapper,
        CommentsDbContext commentsDbContext)
    {
        _logger = logger;
        _mapper = mapper;
        _commentsDbContext = commentsDbContext;
    }

    public async Task<(
        IEnumerable<CommentGetDto> comments,
        MetaData MetaData
        )> GetCommentsAsync(CommentParameters commentParameters)
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

        var metadata = pagedCommentsWithMetaData.MetaData;

        return (commentDtos, metadata);
    }
}
