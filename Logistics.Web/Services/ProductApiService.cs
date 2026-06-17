using System.Globalization;
using Logistics.Web.Models;

namespace Logistics.Web.Services;

public class ProductApiService(ApiClientService apiClient)
{
    public async Task<ApiResponse<List<ProductDto>>?> GetProductsAsync(
        string? search,
        int? categoryId,
        decimal? minPrice = null,
        decimal? maxPrice = null)
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

        if (minPrice.HasValue)
        {
            query.Add($"minPrice={minPrice.Value.ToString(CultureInfo.InvariantCulture)}");
        }

        if (maxPrice.HasValue)
        {
            query.Add($"maxPrice={maxPrice.Value.ToString(CultureInfo.InvariantCulture)}");
        }

        var url = query.Count == 0 ? "api/products" : $"api/products?{string.Join("&", query)}";
        return await apiClient.GetAsync<ApiResponse<List<ProductDto>>>(url);
    }
}
