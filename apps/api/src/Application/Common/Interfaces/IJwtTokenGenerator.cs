using Domain.Iam;

namespace Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string Generate(User user, IEnumerable<string> roles, Guid tenantId, string tenantCode);
}
