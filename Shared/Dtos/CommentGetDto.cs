// Ignore Spelling: Dto
namespace Shared.Dtos;

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

    // Unidirectional tree, no circular links
    // I build the tree on the server
    // And the client just displays the tree, no other actions
    public List<CommentGetDto> Replies { get; set; } = new();
}