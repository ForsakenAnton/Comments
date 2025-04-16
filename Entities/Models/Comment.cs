namespace Entities.Models;

public class Comment
{
    public int Id { get; set; }
    public string Text { get; set; } = "";
    public DateTime CreationDate { get; set; }

    public string? ImageFileName { get; set; }
    public string? TextFileName { get; set; }

    public int? ParentId { get; set; }
    public Comment? Parent { get; set; }
    public ICollection<Comment>? Replies { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }
}