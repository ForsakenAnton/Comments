using Comments.Server.Models.Dtos;
using Microsoft.AspNetCore.Identity;

namespace Comments.Server.Services.Contracts;

public interface IAuthenticationService
{
    Task<IdentityResult> RegisterUserAsync(UserForRegistrationDto userForRegistration);
}