using AutoMapper;
using Comments.Server.Data.Entities;
using Comments.Server.Models.Dtos;
using Comments.Server.Services.Contracts;
using Microsoft.AspNetCore.Identity;

namespace Comments.Server.Services;

internal sealed class AuthenticationService : IAuthenticationService
{
    private readonly ILogger<AuthenticationService> _logger;
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;

    public AuthenticationService(
        ILogger<AuthenticationService> logger,
        IMapper mapper,
        UserManager<User> userManager,
        IConfiguration configuration)
    {
        _logger = logger;
        _mapper = mapper;
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task<IdentityResult> RegisterUserAsync(UserForRegistrationDto userForRegistration)
    {
        User user = _mapper.Map<User>(userForRegistration);

        IdentityResult result = await _userManager.CreateAsync(
            user, 
            userForRegistration.Password);

        return result;
    }
}
