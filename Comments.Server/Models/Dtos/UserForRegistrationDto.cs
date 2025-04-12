using System.ComponentModel.DataAnnotations;

namespace Comments.Server.Models.Dtos;

public record UserForRegistrationDto
{
    public string NickName { get; init; } = "";
    public string? Email { get; init; }

    [Required(ErrorMessage = "Password is required")]
    [MinLength(1)] // just for quick testing
    public string Password { get; init; } = "";

    [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
    public string? ConfirmPassword { get; init; }
}
