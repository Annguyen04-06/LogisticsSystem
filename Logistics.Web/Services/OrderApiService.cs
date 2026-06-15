using System.Net.Http.Json;
using Logistics.Web.Models;

namespace Logistics.Web.Services;

public class OrderApiService(ApiClientService apiClient)
{
    public async Task<ApiResponse<List<OrderDto>>?> CreateOrderAsync(CreateOrderDto request)
    {
        var response = await apiClient.PostAsync("api/orders", request);
        return await response.Content.ReadFromJsonAsync<ApiResponse<List<OrderDto>>>();
    }

    public async Task<ApiResponse<List<OrderDto>>?> GetMyOrdersAsync()
    {
        return await apiClient.GetAsync<ApiResponse<List<OrderDto>>>("api/orders/my-orders");
    }

    public async Task<ApiResponse<OrderDto>?> CancelOrderAsync(int orderId)
    {
        var response = await apiClient.PutAsync($"api/orders/{orderId}/cancel", new { });
        return await response.Content.ReadFromJsonAsync<ApiResponse<OrderDto>>();
    }
}
