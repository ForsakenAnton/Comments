using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class UserRepository : RepositoryBase<User>, IUserRepository
{
    public UserRepository(RepositoryContext repositoryContext)
        : base(repositoryContext)
    {
    }

    public async Task<User?> GetUserByEmailAsync(string email, bool trackChanges)
    {
        User? user = await base
            .FindByConditionWithIncludes(u => u.Email == email, trackChanges)
            .SingleOrDefaultAsync();

        return user;
    }

    public async Task CreateUserAsync(User user)
    {
        await base.CreateAsync(user);
    }
}
