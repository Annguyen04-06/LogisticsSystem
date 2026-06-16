using Logistics.Application.DTOs.Profile;
using Logistics.Domain.Entities;

namespace Logistics.Application.Features.Profile;

public static class ProfileMapping
{
    public static ProfileDto ToDto(User user)
    {
        return new ProfileDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Address = user.Address,
            Role = user.Role,
            IsApproved = user.IsApproved,
            IsActive = user.IsActive,
            AvatarUrl = user.AvatarUrl,
            CreatedAt = user.CreatedAt
        };
    }
}
