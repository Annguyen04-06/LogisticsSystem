using System.Net.Http.Json;
using Logistics.Web.Models;

namespace Logistics.Web.Services;

public class SellerProductApiService(ApiClientService apiClient)
{
    public async Task<ApiResponse<List<ProductDto>>?> GetMyProductsAsync()
    {
        return await apiClient.GetAsync<ApiResponse<List<ProductDto>>>("api/products/my-products");
    }

    public async Task<ApiResponse<ProductDto>?> CreateAsync(CreateProductDto request)
    {
        var response = await apiClient.PostAsync("api/products", request);
        return await response.Content.ReadFromJsonAsync<ApiResponse<ProductDto>>();
    }

    public async Task<ApiResponse<ProductDto>?> UpdateAsync(int id, UpdateProductDto request)
    {
        var response = await apiClient.PutAsync($"api/products/{id}", request);
        return await response.Content.ReadFromJsonAsync<ApiResponse<ProductDto>>();
    }

    public async Task<ApiResponse<bool>?> DeleteAsync(int id)
    {
        var response = await apiClient.DeleteAsync($"api/products/{id}");
        return await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
    }
}
