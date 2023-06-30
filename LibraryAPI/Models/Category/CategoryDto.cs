using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LibraryAPI.Models;

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Priority { get; set; }

    public string CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }
}

