using Entities.Models;

namespace Contracts;

public interface IUserRepository
{
    Task<User?> GetUserByEmailAsync(string email, bool trackChanges);
    Task CreateUserAsync(User user);
}
