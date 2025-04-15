using Contracts;
using Entities.Models;

namespace Repository;

public class CommentRepository : RepositoryBase<Comment>, ICommentRepository
{
    public CommentRepository(RepositoryContext repositoryContext) 
        : base(repositoryContext)
    {
    }
}
