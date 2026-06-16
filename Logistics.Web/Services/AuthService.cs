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
        var currentUser = await tokenStorage.GetCurrentUserAsync();

        if (currentUser != null)
        {
            authState.SetCurrentUser(currentUser);
        }
    }

    public async Task LogoutAsync()
    {
        await tokenStorage.ClearAsync();
        authState.Clear();
    }
}
