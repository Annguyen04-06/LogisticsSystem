using System.Net.Http.Json;
using Logistics.Web.Models;

namespace Logistics.Web.Services;

public class CartApiService(ApiClientService apiClient)
{
    public async Task<ApiResponse<CartDto>?> GetMyCartAsync()
    {
        return await apiClient.GetAsync<ApiResponse<CartDto>>("api/carts/my-cart");
    }

    public async Task<ApiResponse<CartDto>?> AddToCartAsync(AddToCartDto request)
    {
        var response = await apiClient.PostAsync("api/carts/items", request);
        return await response.Content.ReadFromJsonAsync<ApiResponse<CartDto>>();
    }

    public async Task<ApiResponse<CartDto>?> UpdateCartItemAsync(int cartItemId, UpdateCartItemDto request)
    {
        var response = await apiClient.PutAsync($"api/carts/items/{cartItemId}", request);
        return await response.Content.ReadFromJsonAsync<ApiResponse<CartDto>>();
    }

    public async Task<ApiResponse<CartDto>?> RemoveCartItemAsync(int cartItemId)
    {
        var response = await apiClient.DeleteAsync($"api/carts/items/{cartItemId}");
        return await response.Content.ReadFromJsonAsync<ApiResponse<CartDto>>();
    }

    public async Task<ApiResponse<CartDto>?> ClearCartAsync()
    {
        var response = await apiClient.DeleteAsync("api/carts/clear");
        return await response.Content.ReadFromJsonAsync<ApiResponse<CartDto>>();
    }
}
