namespace Entities.ExceptionModels;

public sealed class CommentNotFoundException : NotFoundException
{
    public CommentNotFoundException(int id) 
        : base($"Comment with id: {id} not found.")
    {
    }
}
