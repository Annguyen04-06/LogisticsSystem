using System.Net.Http.Json;
using Logistics.Web.Models;

namespace Logistics.Web.Services;

public class UserApiService(ApiClientService apiClient)
{
    public async Task<ApiResponse<List<UserDto>>?> GetUsersAsync()
    {
        return await apiClient.GetAsync<ApiResponse<List<UserDto>>>("api/users");
    }

    public async Task<ApiResponse<UserDto>?> GetUserByIdAsync(int id)
    {
        return await apiClient.GetAsync<ApiResponse<UserDto>>($"api/users/{id}");
    }

    public async Task<ApiResponse<List<AvailableShipperDto>>?> GetActiveShippersAsync()
    {
        return await apiClient.GetAsync<ApiResponse<List<AvailableShipperDto>>>("api/users/shippers");
    }

    public async Task<ApiResponse<UserDto>?> ApproveSellerAsync(int id)
    {
        var response = await apiClient.PutAsync($"api/users/{id}/approve-seller", new { });
        return await response.Content.ReadFromJsonAsync<ApiResponse<UserDto>>();
    }

    public async Task<ApiResponse<UserDto>?> ToggleUserActiveAsync(int id, bool isActive)
    {
        var response = await apiClient.PutAsync($"api/users/{id}/toggle-active", new UpdateUserStatusDto
        {
            IsActive = isActive
        });

        return await response.Content.ReadFromJsonAsync<ApiResponse<UserDto>>();
    }

    public async Task<ApiResponse<bool>?> DeleteUserAsync(int id)
    {
        var response = await apiClient.DeleteAsync($"api/users/{id}");
        return await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
    }
}
