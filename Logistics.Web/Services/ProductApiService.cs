using Logistics.Web.Models;

namespace Logistics.Web.Services;

public class ProductApiService(ApiClientService apiClient)
{
    public async Task<ApiResponse<List<ProductDto>>?> GetProductsAsync(string? search, int? categoryId)
    {
        var query = new List<string>();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query.Add($"search={Uri.EscapeDataString(search)}");
        }

        if (categoryId.HasValue)
        {
            query.Add($"categoryId={categoryId.Value}");
        }

        var url = query.Count == 0 ? "api/products" : $"api/products?{string.Join("&", query)}";
        return await apiClient.GetAsync<ApiResponse<List<ProductDto>>>(url);
    }
}
