using Logistics.Application.DTOs.Users;
using Logistics.Domain.Entities;

namespace Logistics.Application.Features.Users;

public static class UserMapping
{
    public static UserDto ToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Address = user.Address,
            AvatarUrl = user.AvatarUrl,
            Role = user.Role,
            IsApproved = user.IsApproved,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        };
    }
}
