using System.Net.Http.Json;
using Logistics.Web.Models;

namespace Logistics.Web.Services;

public class DeliveryApiService(ApiClientService apiClient)
{
    public async Task<ApiResponse<OrderDto>?> ConfirmOrderAsync(int orderId)
    {
        var response = await apiClient.PutAsync($"api/deliveries/orders/{orderId}/confirm", new { });
        return await response.Content.ReadFromJsonAsync<ApiResponse<OrderDto>>();
    }

    public async Task<ApiResponse<DeliveryDto>?> AssignShipperAsync(int orderId, AssignShipperDto request)
    {
        var response = await apiClient.PutAsync($"api/deliveries/orders/{orderId}/assign-shipper", request);
        return await response.Content.ReadFromJsonAsync<ApiResponse<DeliveryDto>>();
    }

    public async Task<ApiResponse<List<DeliveryDto>>?> GetShipperOrdersAsync()
    {
        return await apiClient.GetAsync<ApiResponse<List<DeliveryDto>>>("api/deliveries/shipper-orders");
    }

    public async Task<ApiResponse<List<DeliveryDto>>?> GetDeliveryHistoryAsync()
    {
        return await apiClient.GetAsync<ApiResponse<List<DeliveryDto>>>("api/deliveries/history");
    }

    public async Task<ApiResponse<DeliveryDto>?> UpdateDeliveryStatusAsync(int deliveryId, string status)
    {
        var response = await apiClient.PutAsync(
            $"api/deliveries/{deliveryId}/status",
            new UpdateDeliveryStatusDto { Status = status });

        return await response.Content.ReadFromJsonAsync<ApiResponse<DeliveryDto>>();
    }
}
