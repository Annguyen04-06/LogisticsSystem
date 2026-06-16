using System.Net.Http.Json;
using Logistics.Web.Models;

namespace Logistics.Web.Services;

public class ProfileApiService(ApiClientService apiClient)
{
    public async Task<ApiResponse<ProfileDto>?> GetMyProfileAsync()
    {
        return await apiClient.GetAsync<ApiResponse<ProfileDto>>("api/profile/me");
    }

    public async Task<ApiResponse<ProfileDto>?> UpdateProfileAsync(UpdateProfileRequest request)
    {
        var response = await apiClient.PutAsync("api/profile/me", request);
        return await response.Content.ReadFromJsonAsync<ApiResponse<ProfileDto>>();
    }

    public async Task<ApiResponse<bool>?> ChangePasswordAsync(ChangePasswordRequest request)
    {
        var response = await apiClient.PutAsync("api/profile/change-password", request);
        return await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
    }
}
