// Ignore Spelling: Dto

using AutoMapper;
using Contracts;
using Entities.Models;
using Entities.ExceptionModels;
using Service.Contracts;
using Shared.Dtos;
using Shared.RequestFeatures;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Service;

internal sealed class CommentService : ICommentService
{
    private readonly IRepositoryManager _repository;
    private readonly ILoggerManager _logger;
    private readonly IMapper _mapper;

    public CommentService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
    {
        _repository = repository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<(
        IEnumerable<CommentGetDto> comments, 
        MetaData MetaData)> GetAllCommentsWithChildrenAsync(
            CommentParameters commentParameters, 
            bool trackChanges, 
            Expression<Func<Comment, User>>? includeUserExpression)
    {
        PagedList<Comment> pagedCommentsWithMetaData = await _repository.Comment
            .GetAllCommentsWithChildrenAsync(commentParameters, trackChanges, includeUserExpression);

        var commentDtos = _mapper
            .Map<List<CommentGetDto>>(pagedCommentsWithMetaData);

        MetaData metadata = pagedCommentsWithMetaData.MetaData;

        return (commentDtos, metadata);
    }


    public async Task<(
        IEnumerable<CommentGetDto> comments, MetaData MetaData
        )> GetParentCommentsAsync(
            CommentParameters commentParameters,
            bool trackChanges,
            params Expression<Func<Comment, object>>[] includeExpressions)
    {
        PagedList<Comment> pagedCommentsWithMetaData = await _repository.Comment
            .GetParentCommentsAsync(commentParameters, trackChanges, includeExpressions);

        var commentDtos = _mapper
            .Map<List<CommentGetDto>>(pagedCommentsWithMetaData);

        // Make replies count and clear the replies
        commentDtos.ForEach(c =>
        {
            c.ChildrenCommentsCount = c.Replies.Count;
            c.Replies.Clear();
        });

        MetaData metadata = pagedCommentsWithMetaData.MetaData;

        return (commentDtos, metadata);
    }


    public async Task<IEnumerable<CommentGetDto>> GetChildrenCommentsAsync(
        int id, 
        bool trackChanges)
    {
        Comment comment = await GetCommentWithNestedIncludesAndCheckIfExists(id, trackChanges);

        var commentDtos = _mapper
            .Map<List<CommentGetDto>>(comment.Replies);

        // Make replies count and clear the replies
        commentDtos.ForEach(c =>
        {
            c.ChildrenCommentsCount = c.Replies.Count;
            c.Replies.Clear();
        });

        return commentDtos;
    }


    public async Task<CommentGetDto> CreateCommentAsync(
        CommentCreateDto commentDto,
        string? imageFileNameForSave,
        string? textFileNameForSave
        )
    {
        Comment comment = _mapper.Map<Comment>(commentDto);

        await CheckIfRootCommentOrParentCommentExists(comment);

        // manually set image and text filenames, date also
        comment.ImageFileName = imageFileNameForSave;
        comment.TextFileName = textFileNameForSave;
        comment.CreationDate = DateTime.Now;

        User? existingUser = await _repository.User
            .GetUserByEmailAsync(comment.User!.Email, trackChanges: true);

        // User already can be in the DB
        // The logic is the next:
        // If a user with an email not exists,
        // I create him;
        // If a user with an email already exists,
        // I just change him userName and homePage,
        // because I didn't come up with better logic,
        // if the user registers every time to create a comment.
        // In real application, of course, I wouldn't leave it like that...
        if (existingUser is null)
        {
            await _repository.User.CreateUserAsync(comment.User);
            await _repository.SaveAsync();
        }
        else
        {
            // track the existingUser. I don't need a method aka UpdateUser anymore.
            existingUser.UserName = comment.User!.UserName;
            existingUser.HomePage = comment.User!.HomePage;
            await _repository.SaveAsync();

            comment.UserId = existingUser.Id;
            comment.User = null;
        }

        await _repository.Comment.CreateComment(comment);
        await _repository.SaveAsync();

        return _mapper.Map<CommentGetDto>(comment);
    }

    private async Task<Comment> GetCommentAndCheckIfExists(
        int id,
        bool trackChanges)
    {
        Comment? existingComment = await _repository.Comment
            .GetCommentByIdAsync(id, trackChanges);
        if (existingComment is null)
        {
            throw new CommentNotFoundException(id);
        }

        return existingComment;
    }

    private async Task<Comment> GetCommentWithNestedIncludesAndCheckIfExists(
    int id,
    bool trackChanges,
    params Expression<Func<Comment, object>>[] includeExpressions)
    {
        Comment? existingComment = await _repository.Comment
            .GetCommentByIdWithNestedIncludes(id, trackChanges);
        if (existingComment is null)
        {
            throw new CommentNotFoundException(id);
        }

        return existingComment;
    }

    private async Task CheckIfRootCommentOrParentCommentExists(Comment comment)
    {
        if (comment.ParentId is null)
        {
            return; // Ok. Comment is a root comment
        }

        Comment? parentComment = await _repository.Comment
            .GetCommentByIdAsync(comment.ParentId.Value, trackChanges: false);

        if (parentComment is null)
        {
            throw new CommentNotFoundException(comment.ParentId.Value);
        }
    }
}