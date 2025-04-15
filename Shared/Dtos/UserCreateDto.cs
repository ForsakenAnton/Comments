// Ignore Spelling: Dto

using System.ComponentModel.DataAnnotations;

namespace Shared.Dtos;

public class UserCreateDto
{
    [Required]
    [RegularExpression(@"^[a-zA-Z0-9\s]+$")]
    public string UserName { get; set; } = "";

    [Required]
    [EmailAddress]
    public string Email { get; set; } = "";

    [Url]
    public string HomePage { get; set; } = "";
}
