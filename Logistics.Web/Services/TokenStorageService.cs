using Logistics.Web.Models;
using Microsoft.JSInterop;

namespace Logistics.Web.Services;

public class TokenStorageService(IJSRuntime jsRuntime)
{
    private const string TokenKey = "logistics_token";
    private const string UserIdKey = "logistics_user_id";
    private const string FullNameKey = "logistics_full_name";
    private const string EmailKey = "logistics_email";
    private const string RoleKey = "logistics_role";

    public async Task SaveAsync(CurrentUser user)
    {
        await jsRuntime.InvokeVoidAsync("localStorage.setItem", TokenKey, user.Token);
        await jsRuntime.InvokeVoidAsync("localStorage.setItem", UserIdKey, user.UserId.ToString());
        await jsRuntime.InvokeVoidAsync("localStorage.setItem", FullNameKey, user.FullName);
        await jsRuntime.InvokeVoidAsync("localStorage.setItem", EmailKey, user.Email);
        await jsRuntime.InvokeVoidAsync("localStorage.setItem", RoleKey, user.Role);
    }

    public async Task<CurrentUser?> GetCurrentUserAsync()
    {
        var token = await jsRuntime.InvokeAsync<string?>("localStorage.getItem", TokenKey);

        if (string.IsNullOrWhiteSpace(token))
        {
            return null;
        }

        var userIdValue = await jsRuntime.InvokeAsync<string?>("localStorage.getItem", UserIdKey);
        var fullName = await jsRuntime.InvokeAsync<string?>("localStorage.getItem", FullNameKey);
        var email = await jsRuntime.InvokeAsync<string?>("localStorage.getItem", EmailKey);
        var role = await jsRuntime.InvokeAsync<string?>("localStorage.getItem", RoleKey);

        return new CurrentUser
        {
            UserId = int.TryParse(userIdValue, out var userId) ? userId : 0,
            FullName = fullName ?? string.Empty,
            Email = email ?? string.Empty,
            Role = role ?? string.Empty,
            Token = token
        };
    }

    public async Task ClearAsync()
    {
        await jsRuntime.InvokeVoidAsync("localStorage.removeItem", TokenKey);
        await jsRuntime.InvokeVoidAsync("localStorage.removeItem", UserIdKey);
        await jsRuntime.InvokeVoidAsync("localStorage.removeItem", FullNameKey);
        await jsRuntime.InvokeVoidAsync("localStorage.removeItem", EmailKey);
        await jsRuntime.InvokeVoidAsync("localStorage.removeItem", RoleKey);
    }
}
