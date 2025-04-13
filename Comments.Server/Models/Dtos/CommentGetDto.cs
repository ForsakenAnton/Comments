// Ignore Spelling: Dto

using Comments.Server.Data.Entities;

namespace Comments.Server.Models.Dtos;

public class CommentGetDto
{
    public int Id { get; set; }
    public string Text { get; set; } = "";

    public DateTime CreationDate { get; set; }

    public string? ImageFile { get; set; }
    public string? TextFile { get; set; }


    public int? ParentId { get; set; }
    public int UserId { get; set; }

    public UserGetDto? User { get; set; }
}