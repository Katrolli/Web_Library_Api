using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryAPI.Models;

public class RegisterModel
{
    public string Name { get; set; }
    public string UserName { get; set; }

    public string Email { get; set; }


    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? Password { get; set; }
    public string? Bio { get; set; }
    public int RoleId { get; set; }
}