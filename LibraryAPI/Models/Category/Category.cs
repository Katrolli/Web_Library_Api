using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LibraryAPI.Models;

[Table("Categories")]
public class Category
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public int Priority { get; set; }
    public List<BookCategory> Books { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("CreatedBy")]
    public int CreatedById { get; set; }
    public string CreatedBy { get; set; }
}

