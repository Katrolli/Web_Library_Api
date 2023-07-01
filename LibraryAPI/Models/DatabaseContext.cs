using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LibraryAPI.Models;
public partial class DatabaseContext : IdentityDbContext<User, Role, int>
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options)
           : base(options)
    {
    }
    public virtual DbSet<Category> Categories { get; set; }
    public virtual DbSet<Book> Books { get; set; }
    public virtual DbSet<BookCategory> BookCategories { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySql("server=localhost;database=librarydb;user=user;password=password", new MySqlServerVersion(new Version(8, 0, 33)));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        base.OnModelCreating(modelBuilder);

        // user role
        modelBuilder.Entity<User>()
            .HasOne(u => u.Role)
            .WithMany()
            .HasForeignKey(u => u.RoleId);

        // author books
        modelBuilder.Entity<Book>()
            .HasOne(b => b.Author)
            .WithMany(u => u.Books)
            .HasForeignKey(b => b.AuthorId);

        // modelBuilder.Entity<Book>()
        //    .HasOne(b => b.CreatedBy)
        //    .WithMany()
        //    .HasForeignKey(b => b.CreatedById);

        // Book Categories
        modelBuilder.Entity<BookCategory>()
            .HasKey(bc => new { bc.BookId, bc.CategoryId });

        modelBuilder.Entity<BookCategory>()
            .HasOne(bc => bc.Book)
            .WithMany(b => b.Categories)
            .HasForeignKey(bc => bc.BookId);

        modelBuilder.Entity<BookCategory>()
            .HasOne(bc => bc.Category)
            .WithMany(c => c.Books)
            .HasForeignKey(bc => bc.CategoryId);
    }
}