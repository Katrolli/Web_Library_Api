using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LibraryAPI.Models;

public class CreateCategory
{
    public string Name { get; set; }
    public int Priority { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

}

