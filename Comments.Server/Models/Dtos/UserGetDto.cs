// Ignore Spelling: Dto

using Comments.Server.Data.Entities;

namespace Comments.Server.Models.Dtos;

public class UserGetDto
{
    public int Id { get; set; }
    public string UserName { get; set; } = "";
    public string Email { get; set; } = "";
    public string HomePage { get; set; } = "";
}
