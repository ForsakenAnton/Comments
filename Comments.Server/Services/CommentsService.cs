// Ignore Spelling: Dto

using AutoMapper;
using Comments.Server.Controllers;
using Comments.Server.Data;
using Comments.Server.Data.Entities;
using Comments.Server.Models.Dtos;
using Comments.Server.Models.RequestFeatures;
using Comments.Server.Models.ValidationModels;
using Comments.Server.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Comments.Server.Services;

public class CommentsService : ICommentsService
{
    private readonly ILogger<CommentsController> _logger;
    private readonly IMapper _mapper;
    private readonly CommentsDbContext _commentsDbContext;
    private readonly IGenerateFileNameService _generatefileNameService;
    private readonly IImageFileService _imageFileService;
    private readonly ITextFileService _textFileService;
    private readonly IWebHostEnvironment _environment;
    private readonly CommentTextValidator _textValidator;

    public CommentsService(
        ILogger<CommentsController> logger,
        IMapper mapper,
        CommentsDbContext commentsDbContext,
        IGenerateFileNameService fileNameService,
        IImageFileService imageService,
        ITextFileService textFileService,
        IWebHostEnvironment environment)
    {
        _logger = logger;
        _mapper = mapper;
        _commentsDbContext = commentsDbContext;
        _generatefileNameService = fileNameService;
        _imageFileService = imageService;
        _textFileService = textFileService;
        _environment = environment;

        _textValidator = new CommentTextValidator();
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


    public async Task<CommentGetDto> CreateCommentAsync(CommentCreateDto commentDto)
    {
        //var existingUser = await _commentsDbContext.Users
        //    .FirstOrDefaultAsync(u => u.Email == commentDto.Email);

        //if (existingUser is null)
        //{
        //    await _commentsDbContext.Users.AddAsync(existingUser);
        //    await _commentsDbContext.SaveChangesAsync();
        //}

        string sanitizedText = _textValidator.SanitizeText(commentDto.Text);

        string? imageFileName = null;
        string? textFileName = null;

        if (commentDto.ImageFile is not null)
        {
            imageFileName = await _generatefileNameService.GenerateFileName(commentDto.ImageFile);
            await _imageFileService.ResizeAndSaveImageAsync(commentDto.ImageFile, imageFileName);
        }
        if (commentDto.TextFile is not null)
        {
            textFileName = await _generatefileNameService.GenerateFileName(commentDto.TextFile);
            await _textFileService.SaveFileAsync(commentDto.TextFile, textFileName);

        }

        string applicationUrl = _environment.EnvironmentName == "Development" ?
            "https://localhost:7092" :
            "https://qwerty123";

        string? imageFileServerPath = imageFileName is not null
            ? $"{applicationUrl}/images/{imageFileName}"
            : null;

        string? textFileServerPath = textFileName is not null
            ? $"{applicationUrl}/textFiles/{textFileName}"
            : null;


        Comment comment = _mapper.Map<Comment>(commentDto);

        comment.ImageFile = imageFileServerPath;
        comment.TextFile = textFileServerPath;
        comment.Text = sanitizedText;

        User? existingUser = await _commentsDbContext.Users
            .FirstOrDefaultAsync(u => u.Email == comment.User!.Email);

        if (existingUser is null)
        {
            await _commentsDbContext.Users.AddAsync(comment.User!);
            await _commentsDbContext.SaveChangesAsync();
        }
        else
        {
            existingUser.UserName = comment.User!.UserName;
            existingUser.HomePage = comment.User!.HomePage;
            //_commentsDbContext.Users.Update(existingUser);
            await _commentsDbContext.SaveChangesAsync();

            comment.UserId = existingUser.Id;
            comment.User = null;
        }

        await _commentsDbContext.Comments.AddAsync(comment);
        await _commentsDbContext.SaveChangesAsync();

        return _mapper.Map<CommentGetDto>(comment);
    }
}
