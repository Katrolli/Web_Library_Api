using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace LibraryAPI.Models;

[Table("Roles")]
public class Role : IdentityRole<int>
{
}