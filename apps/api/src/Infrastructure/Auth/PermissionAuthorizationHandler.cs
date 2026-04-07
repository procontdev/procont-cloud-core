using Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Infrastructure.Auth;

public sealed class PermissionAuthorizationHandler(IPermissionRepository permissionRepository) : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var userIdClaim = context.User.Claims.FirstOrDefault(x => x.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return;
        }

        var permissions = await permissionRepository.GetPermissionKeysByUserIdAsync(userId);
        if (permissions.Contains(requirement.Permission, StringComparer.OrdinalIgnoreCase))
        {
            context.Succeed(requirement);
        }
    }
}
