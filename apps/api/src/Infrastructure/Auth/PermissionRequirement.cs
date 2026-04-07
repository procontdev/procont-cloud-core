using Microsoft.AspNetCore.Authorization;

namespace Infrastructure.Auth;

public sealed record PermissionRequirement(string Permission) : IAuthorizationRequirement;
