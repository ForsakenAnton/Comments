namespace Contracts;

public interface IRepositoryManager
{
    IUserRepository User { get; }
    ICommentRepository Comment { get; } 

    void Save();
    Task SaveAsync();
}
