using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Iam;

namespace Application.Auth;

public sealed class AuthService(
    IUserRepository userRepository,
    ITenantContext tenantContext,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator jwtTokenGenerator,
    IActionAuditLogger actionAuditLogger)
{
    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            throw new ValidationException("Credenciales invalidas.");
        }

        var user = await userRepository.GetByEmailAsync(tenantContext.TenantId, request.Email.Trim().ToLowerInvariant(), cancellationToken);
        if (user is null || user.Status != UserStatus.Active || !passwordHasher.Verify(user.PasswordHash, request.Password))
        {
            throw new ValidationException("Credenciales invalidas.");
        }

        var roles = await userRepository.GetRoleNamesAsync(user.Id, cancellationToken);
        var token = jwtTokenGenerator.Generate(user, roles, tenantContext.TenantId, tenantContext.TenantCode);

        await actionAuditLogger.LogAsync("auth.login", nameof(User), user.Id.ToString(), cancellationToken: cancellationToken);

        return new LoginResponse(token, "Bearer", DateTime.UtcNow.AddHours(8), roles.ToArray());
    }
}
