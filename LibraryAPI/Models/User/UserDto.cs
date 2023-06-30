public class UserDto
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public string? Bio { get; set; }
    public string Role { get; set; }
    public string CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }

    public IEnumerable<BookDto> Books { get; set; }
}