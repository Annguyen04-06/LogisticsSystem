using Logistics.Domain.Common;
using Logistics.Domain.Enums;

namespace Logistics.Domain.Entities;

public class User : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public bool IsApproved { get; set; }
    public bool IsActive { get; set; } = true;
}
