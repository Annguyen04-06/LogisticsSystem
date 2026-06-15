using Logistics.Web.Models;

namespace Logistics.Web.Services;

public class SellerOrderApiService(ApiClientService apiClient)
{
    public async Task<ApiResponse<List<OrderDto>>?> GetSellerOrdersAsync()
    {
        return await apiClient.GetAsync<ApiResponse<List<OrderDto>>>("api/deliveries/seller-orders");
    }
}
