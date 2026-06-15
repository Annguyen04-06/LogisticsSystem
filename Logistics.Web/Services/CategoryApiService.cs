using Logistics.Web.Models;

namespace Logistics.Web.Services;

public class CategoryApiService(ApiClientService apiClient)
{
    public async Task<ApiResponse<List<CategoryDto>>?> GetCategoriesAsync()
    {
        return await apiClient.GetAsync<ApiResponse<List<CategoryDto>>>("api/categories");
    }
}
