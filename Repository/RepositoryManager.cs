using Contracts;

namespace Repository;

public sealed class RepositoryManager : IRepositoryManager
{
    private readonly RepositoryContext _repositoryContext;
    private readonly Lazy<IUserRepository> _userRepository;
    private readonly Lazy<ICommentRepository> _commentRepository;

    public RepositoryManager(RepositoryContext repositoryContext)
    {
        _repositoryContext = repositoryContext;

        _userRepository = new Lazy<IUserRepository>(() =>
            new UserRepository(repositoryContext));

        _commentRepository = new Lazy<ICommentRepository>(() =>
            new CommentRepository(repositoryContext));
    }

    public IUserRepository User => _userRepository.Value;
    public ICommentRepository Comment => _commentRepository.Value;

    public void Save() => _repositoryContext.SaveChanges();
    public async Task SaveAsync() => await _repositoryContext.SaveChangesAsync();
}
