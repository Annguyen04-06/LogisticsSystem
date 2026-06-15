using Logistics.Domain.Entities;

namespace Logistics.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
}
