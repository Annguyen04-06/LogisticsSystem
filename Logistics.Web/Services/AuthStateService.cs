using Logistics.Web.Models;

namespace Logistics.Web.Services;

public class AuthStateService
{
    public event Action? OnChange;

    public CurrentUser? CurrentUser { get; private set; }
    public string? Token => CurrentUser?.Token;
    public string DisplayName => CurrentUser?.FullName ?? "Khách";
    public string CurrentRole => CurrentUser?.Role ?? "Guest";
    public bool IsAuthenticated => CurrentUser != null && !string.IsNullOrWhiteSpace(CurrentUser.Token);

    public void SetCurrentUser(CurrentUser user)
    {
        CurrentUser = user;
        NotifyStateChanged();
    }

    public void UpdateProfile(string fullName, string? avatarUrl)
    {
        if (CurrentUser is null)
        {
            return;
        }

        CurrentUser.FullName = fullName;
        CurrentUser.AvatarUrl = avatarUrl;
        NotifyStateChanged();
    }

    public void Clear()
    {
        CurrentUser = null;
        NotifyStateChanged();
    }

    public static string GetVietnameseRoleName(string? role)
    {
        return role switch
        {
            "Customer" => "Khách hàng",
            "Seller" => "Người bán",
            "Shipper" => "Người giao hàng",
            "Admin" => "Quản trị viên",
            _ => "Khách"
        };
    }

    private void NotifyStateChanged()
    {
        OnChange?.Invoke();
    }
}
