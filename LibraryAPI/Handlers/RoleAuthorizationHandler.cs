using Microsoft.AspNetCore.Authorization;
using LibraryAPI.Models;
using Microsoft.AspNetCore.Identity;

public class RoleAuthorizationHandler : AuthorizationHandler<RoleRequirement>
{
    private readonly UserManager<User> _userManager;

    public RoleAuthorizationHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleRequirement requirement)
    {
        var user = await _userManager.GetUserAsync(context.User);

        if (user != null && user.RoleId == requirement.Role.Id)
        {
            context.Succeed(requirement);
        }
    }
}
