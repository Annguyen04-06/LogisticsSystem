using Logistics.Web.Models;

namespace Logistics.Web.Services;

public class AdminDashboardApiService(ApiClientService apiClient)
{
    public async Task<ApiResponse<AdminDashboardDto>?> GetDashboardAsync()
    {
        return await apiClient.GetAsync<ApiResponse<AdminDashboardDto>>("api/dashboard/admin");
    }

    public async Task<ApiResponse<List<OrderStatisticsByDateDto>>?> GetOrdersByDateAsync()
    {
        return await apiClient.GetAsync<ApiResponse<List<OrderStatisticsByDateDto>>>("api/dashboard/orders-by-date");
    }

    public async Task<ApiResponse<List<OrderStatisticsByMonthDto>>?> GetOrdersByMonthAsync()
    {
        return await apiClient.GetAsync<ApiResponse<List<OrderStatisticsByMonthDto>>>("api/dashboard/orders-by-month");
    }
}
