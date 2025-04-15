// Ignore Spelling: Dto Captcha

using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Shared.Dtos;

public class CommentCreateDto
{
    [Required]
    public string Text { get; set; } = "";

    //public DateTime CreationDate { get; set; } = DateTime.Now;

    public IFormFile? ImageFile { get; set; }
    public IFormFile? TextFile { get; set; }

    [Required]
    [RegularExpression(
        @"^[a-zA-Z0-9]+$", 
        ErrorMessage = "CAPTCHA can contain only Latin letters and numbers"
    )]
    public string Captcha { get; set; } = "";

    public int? ParentId { get; set; }

    public UserCreateDto? User { get; set; }
}
