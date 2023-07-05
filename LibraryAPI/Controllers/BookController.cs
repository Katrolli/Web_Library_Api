using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryAPI.Models;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Cors;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BookController : ControllerBase
    {
        private readonly DatabaseContext _dbContext;
        private readonly UserManager<User> _userManager;

        public BookController(DatabaseContext dbContext, UserManager<User> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDto>>> Get()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var booksQuery = _dbContext.Books
                .Include(b => b.Author)
                .Include(b => b.Categories)
                    .ThenInclude(bc => bc.Category);

            // Get the current user's ID
            var userId = int.Parse(_userManager.GetUserId(User));

            // Check if the user is an admin
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin)
            {
                // Filter the books for the logged-in author only
                booksQuery = booksQuery.Where(b => b.AuthorId == userId)
                    .Include(b => b.Author)
                    .Include(b => b.Categories)
                        .ThenInclude(bc => bc.Category);
            }

            var books = await booksQuery.ToListAsync();

            var bookDtos = books.Select(book => new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                Description = book.Description,
                Author = book.Author.Name,
                AuthorId = book.Author.Id,
                ImageUrl = book.ImageUrl,
                CreatedAt = book.CreatedAt,
                CreatedBy = book.CreatedBy,
                Categories = book.Categories.Select(category => category.Category.Name).ToList()
            }).ToList();

            return Ok(bookDtos);
        }



        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var book = _dbContext.Books.Include(b => b.Categories).Include(b => b.Author).FirstOrDefault(b => b.Id == id);

            if (book == null)
            {
                return NotFound();
            }
            var bookDto = new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                Description = book.Description,
                Author = book.Author.Name,
                CreatedBy = book.CreatedBy,
                Categories = book.Categories.Select(category => category.Category.Name).ToList()
            };
            return Ok(bookDto);
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            Console.WriteLine(id);
            Book? book = _dbContext.Books.Find(id);
            if (book != null)
            {
                _dbContext.Books.Remove(book);
                _dbContext.SaveChanges();
                return Ok("BookRemoved");
            }
            else
            {
                throw new ArgumentNullException();
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Book>> CreateAsync([FromForm] CreateBook createBook)
        {
            // var book = _dbContext.Books.Where(b => b.id == createBook.Title);
            // if (book != null)
            // {
            //     return Conflict("A book with the same title already exists."); ;
            // }
            var imageUrl = "";
            // Process the image file
            if (createBook.Cover != null && createBook.Cover.Length > 0)
            {
                // Generate a unique file name for the image
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + createBook.Cover.FileName;

                // Define the path where the image will be saved
                string imagePath = Path.Combine("images", uniqueFileName);

                // Save the image to the file system
                using (var fileStream = new FileStream(imagePath, FileMode.Create))
                {
                    await createBook.Cover.CopyToAsync(fileStream);
                }

                // Set the image URL for the book
                imageUrl = imagePath;
            }

            var currentUser = await _userManager.GetUserAsync(User);
            Console.Write(currentUser);
            Book newBook = new Book
            {
                Title = createBook.Title,
                ImageUrl = imageUrl,
                Description = createBook.Description,
                AuthorId = createBook.AuthorId,
                Categories = new List<BookCategory>(),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = currentUser.Name
            };
            foreach (var categoryId in createBook.CategoryIds)
            {
                newBook.Categories.Add(new BookCategory
                {
                    BookId = newBook.Id,
                    CategoryId = categoryId
                });
            }
            await _dbContext.Books.AddAsync(newBook);
            await _dbContext.SaveChangesAsync();
            return await Task.FromResult(newBook);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Book>> Put(int id, [FromForm] EditBook createBook)
        {
            // Check if the user is an admin

            var book = await _dbContext.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            // Process the image file
            if (createBook.Cover != null && createBook.Cover.Length > 0)
            {
                // Generate a unique file name for the image
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + createBook.Cover.FileName;

                // Define the path where the image will be saved
                string imagePath = Path.Combine("images", uniqueFileName);

                // Save the image to the file system
                using (var fileStream = new FileStream(imagePath, FileMode.Create))
                {
                    await createBook.Cover.CopyToAsync(fileStream);
                }

                // Set the image URL for the book
                book.ImageUrl = imagePath;

            }
            book.Title = createBook.Title;
            book.Description = createBook.Description;
            book.AuthorId = createBook.AuthorId;

            _dbContext.Entry(book).Collection(b => b.Categories).Load();



            _dbContext.BookCategories.RemoveRange(book.Categories);

            foreach (var categoryId in createBook.CategoryIds)
            {
                book.Categories.Add(new BookCategory
                {
                    BookId = book.Id,
                    CategoryId = categoryId
                });
            }
            await _dbContext.SaveChangesAsync();

            return book;
        }
    }
}