using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
namespace LibraryAPI.Models;

[Table("Users")]
public class User : IdentityUser<int>
{
    public string Name { get; set; }
    public string? Bio { get; set; }
    [ForeignKey("Role")]
    public int RoleId { get; set; }
    public string? RefreshToken { get; set; }
    [ForeignKey("CreatedBy")]
    public int CreatedById { get; set; }
    public string CreatedBy { get; set; }
    public Role Role { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<Book> Books { get; set; }
}

