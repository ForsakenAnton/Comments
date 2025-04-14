// Ignore Spelling: Dto

using Comments.Server.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace Comments.Server.Models.Dtos;

public class UserGetDto
{
    public int Id { get; set; }

    //[Required]
    //[RegularExpression(@"^[a-zA-Z0-9\s]+$")]
    public string UserName { get; set; } = "";

    //[Required]
    //[EmailAddress]
    public string Email { get; set; } = "";
    public string HomePage { get; set; } = "";
}
