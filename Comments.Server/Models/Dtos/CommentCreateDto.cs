// Ignore Spelling: Dto Captcha

using System.ComponentModel.DataAnnotations;

namespace Comments.Server.Models.Dtos;

public class CommentCreateDto
{
    [Required]
    public string Text { get; set; } = "";

    public DateTime CreationDate { get; set; } = DateTime.Now;

    public IFormFile? ImageFile { get; set; }
    public IFormFile? TextFile { get; set; }

    [Required]
    [RegularExpression(
        @"^[a-zA-Z0-9]+$", 
        ErrorMessage = "CAPTCHA can contain only Latin letters and numbers"
    )]
    public string CaptchaText { get; set; } = "";



    public int? ParentId { get; set; }
    public int UserId { get; set; }

    public UserCreateDto? User { get; set; }
}
