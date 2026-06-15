using Logistics.Domain.Common;

namespace Logistics.Domain.Entities;

public class Wallet : BaseEntity
{
    public int UserId { get; set; }
    public decimal Balance { get; set; }
}
