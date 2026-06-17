using System.Net.Http.Json;
using Logistics.Web.Models;

namespace Logistics.Web.Services;

public class CategoryApiService(ApiClientService apiClient)
{
    public async Task<ApiResponse<List<CategoryDto>>?> GetCategoriesAsync(bool includeInactive = false)
    {
        var url = includeInactive ? "api/categories?includeInactive=true" : "api/categories";
        return await apiClient.GetAsync<ApiResponse<List<CategoryDto>>>(url);
    }

    public async Task<ApiResponse<CategoryDto>?> CreateAsync(CreateCategoryDto request)
    {
        var response = await apiClient.PostAsync("api/categories", request);
        return await response.Content.ReadFromJsonAsync<ApiResponse<CategoryDto>>();
    }

    public async Task<ApiResponse<CategoryDto>?> UpdateAsync(int id, UpdateCategoryDto request)
    {
        var response = await apiClient.PutAsync($"api/categories/{id}", request);
        return await response.Content.ReadFromJsonAsync<ApiResponse<CategoryDto>>();
    }

    public async Task<ApiResponse<bool>?> DeleteAsync(int id)
    {
        var response = await apiClient.DeleteAsync($"api/categories/{id}");
        return await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
    }
}
