using System.Net.Http.Json;
using Logistics.Web.Models;

namespace Logistics.Web.Services;

public class AuthService(
    ApiClientService apiClient,
    TokenStorageService tokenStorage,
    AuthStateService authState)
{
    public async Task<ApiResponse<AuthResponse>?> LoginAsync(LoginRequest request)
    {
        var httpResponse = await apiClient.PostAsync("api/Auth/login", request);
        var response = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<AuthResponse>>();

        if (httpResponse.IsSuccessStatusCode && response?.Success == true && response.Data != null)
        {
            var currentUser = new CurrentUser
            {
                UserId = response.Data.UserId,
                FullName = response.Data.FullName,
                Email = response.Data.Email,
                Role = response.Data.Role,
                Token = response.Data.Token
            };

            await tokenStorage.SaveAsync(currentUser);
            authState.SetCurrentUser(currentUser);

            try
            {
                await RefreshCurrentUserAsync();
            }
            catch
            {
                // Profile refresh enriches the session with AvatarUrl, but login should still succeed without it.
            }
        }

        return response;
    }

    public async Task<ApiResponse<AuthResponse>?> RegisterAsync(RegisterRequest request)
    {
        var httpResponse = await apiClient.PostAsync("api/Auth/register", request);
        return await httpResponse.Content.ReadFromJsonAsync<ApiResponse<AuthResponse>>();
    }

    public async Task<ApiResponse<ForgotPasswordResponse>?> ForgotPasswordAsync(string email)
    {
        var request = new ForgotPasswordRequest
        {
            Email = email
        };

        var httpResponse = await apiClient.PostAsync("api/Auth/forgot-password", request);
        return await httpResponse.Content.ReadFromJsonAsync<ApiResponse<ForgotPasswordResponse>>();
    }

    public async Task<ApiResponse<string>?> ResetPasswordAsync(ResetPasswordRequest request)
    {
        var httpResponse = await apiClient.PostAsync("api/Auth/reset-password", request);
        return await httpResponse.Content.ReadFromJsonAsync<ApiResponse<string>>();
    }

    public async Task LoadSessionAsync()
    {
        await authState.LoadCurrentUserAsync();
    }

    public async Task RefreshCurrentUserAsync()
    {
        if (!authState.IsAuthenticated)
        {
            return;
        }

        var response = await apiClient.GetAsync<ApiResponse<ProfileDto>>("api/profile/me");

        if (response?.Success == true && response.Data is not null)
        {
            await UpdateCurrentUserFromProfileAsync(response.Data);
        }
    }

    public async Task UpdateCurrentUserFromProfileAsync(ProfileDto profile)
    {
        if (authState.CurrentUser is null)
        {
            return;
        }

        var currentUser = new CurrentUser
        {
            UserId = profile.Id,
            FullName = profile.FullName,
            Email = profile.Email,
            Role = profile.Role,
            Token = authState.CurrentUser.Token,
            AvatarUrl = NormalizeImageUrl(profile.AvatarUrl)
        };

        await tokenStorage.SaveAsync(currentUser);
        authState.SetCurrentUser(currentUser);
    }

    public async Task LogoutAsync()
    {
        await tokenStorage.ClearAsync();
        authState.Clear();
    }

    private static string? NormalizeImageUrl(string? imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            return null;
        }

        if (Uri.TryCreate(imageUrl, UriKind.Absolute, out _))
        {
            return imageUrl;
        }

        return $"http://localhost:5203/{imageUrl.TrimStart('/')}";
    }
}
