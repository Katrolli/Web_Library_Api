using Microsoft.AspNetCore.Authorization;
using LibraryAPI.Models;

public class RoleRequirement : IAuthorizationRequirement
{
    public Role Role { get; }

    public RoleRequirement(Role role)
    {
        Role = role;
    }
}