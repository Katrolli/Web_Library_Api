using Microsoft.AspNetCore.Http;
namespace LibraryAPI.Models;

public class BaseBook
{
    public string Title { get; set; }
    public string Description { get; set; }
    public int AuthorId { get; set; }
    public List<int> CategoryIds { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class CreateBook : BaseBook
{
    public IFormFile Cover { get; set; }
}



public class EditBook : BaseBook
{
    public IFormFile? Cover { get; set; }
}
