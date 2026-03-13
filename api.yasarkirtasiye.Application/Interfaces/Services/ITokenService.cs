using api.yasarkirtasiye.Domain.Entities;

namespace api.yasarkirtasiye.Application.Interfaces.Services;

public interface ITokenService
{
    Task<string> GenerateTokenAsync(User user, IList<string> roles);
}
