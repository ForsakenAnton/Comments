// Ignore Spelling: Dto

using AutoMapper;
using Contracts;
using Entities.Models;
using Service.Contracts;
using Shared.Dtos;
using Shared.RequestFeatures;
using System.Linq.Expressions;

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
        MetaData MetaData)> GetCommentsAsync(
            CommentParameters commentParameters, 
            bool trackChanges, 
            Expression<Func<Comment, User>>? includeUserExpression)
    {
        var pagedCommentsWithMetaData = await _repository.Comment
            .GetCommentsAsync(commentParameters, trackChanges, includeUserExpression);

        var commentDtos = _mapper
            .Map<List<CommentGetDto>>(pagedCommentsWithMetaData);

        var metadata = pagedCommentsWithMetaData.MetaData;

        return (commentDtos, metadata);
    }


    public async Task<CommentGetDto> CreateCommentAsync(
        CommentCreateDto commentDto,
        string? imageFileNameForSave,
        string? textFileNameForSave
        )
    {
        Comment comment = _mapper.Map<Comment>(commentDto);

        // manually set image and text filenames
        comment.ImageFileName = imageFileNameForSave;
        comment.TextFileName = textFileNameForSave;

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
}