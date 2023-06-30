using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryAPI.Models;
using Microsoft.AspNetCore.Identity;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly DatabaseContext _dbContext;
        private readonly UserManager<User> _userManager;

        public UserController(DatabaseContext dbContext, UserManager<User> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> Get()
        {
            var users = await _dbContext.Users
                .Include(u => u.Role)
                .Include(u => u.Books) // Include the Books collection
                .ToListAsync();

            var userDtos = users.Select(user => new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                UserName = user.UserName,
                Email = user.Email,
                Bio = user.Bio,
                CreatedAt = user.CreatedAt,
                CreatedBy = user.CreatedBy,
                Role = user.Role?.Name, // Use the null conditional operator to handle potential null reference
                Books = user.Books?.Select(book => new BookDto
                {
                    Id = book.Id,
                    Title = book.Title,
                    Description = book.Description,
                    Author = book.Author?.Name ?? string.Empty, // Handle potential null reference for Author and its Name property
                    ImageUrl = book.ImageUrl,
                    Categories = book.Categories?.Select(category => category.Category.Name).ToList() ?? new List<string>() // Handle potential null reference for Categories
                })
            }).ToList();

            return Ok(userDtos);
        }



        [HttpGet("{id}")]
        public ActionResult<User> Get(int id)
        {
            var user = _dbContext.Users
            .Include(u => u.Role)
            .Include(u => u.Books)
            .ThenInclude(ub => ub.Categories)
            .FirstOrDefault(u => u.Id == id);

            if (user == null)
            {
                return NotFound("User not found");
            }
            var userDto = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Bio = user.Bio,
                Role = user.Role.Name,
                Books = user.Books.Select(book => new BookDto
                {
                    Id = book.Id,
                    Title = book.Title,
                    Description = book.Description,
                    Author = book.Author.Name,
                    ImageUrl = book.ImageUrl,

                })
            };
            return Ok(userDto);
        }

        [HttpDelete("{id}")]
        [Authorize("AdminOnly")]
        public User Delete(int id)
        {
            User? user = _dbContext.Users.Find(id);
            if (user != null)
            {
                _dbContext.Users.Remove(user);
                _dbContext.SaveChanges();
                return user;
            }
            else
            {
                throw new ArgumentNullException();
            }
        }

        [HttpPost]
        [Authorize("AdminOnly")]
        public async Task<ActionResult<User>> Post(RegisterModel user)
        {
            var userExists = await _userManager.FindByEmailAsync(user.Email);
            var currentUser = await _userManager.GetUserAsync(User);
            if (userExists == null)
            {
                User newUser = new User { Name = user.Name, Email = user.Email, UserName = user.UserName, RoleId = user.RoleId, Bio = user.Bio, CreatedBy = currentUser.Name };
                var result = await _userManager.CreateAsync(newUser, user.Password);

                if (result.Succeeded)
                {
                    return await Task.FromResult(newUser);
                }
                else
                {
                    var errors = result.Errors.Select(e => e.Description);
                    Console.WriteLine("Failed to create user. Errors: " + string.Join(", ", errors));
                }
            }
            return BadRequest("User already exists.");
        }

        [HttpPut("{id}")]
        [Authorize("AdminOnly")]
        public async Task<ActionResult<User>> Put(string id, RegisterModel updatedUser)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Update the user properties with the provided values
            user.Name = updatedUser.Name;
            user.Email = updatedUser.Email;
            user.UserName = updatedUser.Email;
            user.Bio = updatedUser.Bio;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return user;
            }
            else
            {
                var errors = result.Errors.Select(e => e.Description);
                Console.WriteLine("Failed to update user. Errors: " + string.Join(", ", errors));
                return StatusCode(500, "Failed to update user.");
            }
        }


    }
}

