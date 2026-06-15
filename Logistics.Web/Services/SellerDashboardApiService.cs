using Logistics.Web.Models;

namespace Logistics.Web.Services;

public class SellerDashboardApiService(ApiClientService apiClient)
{
    public async Task<ApiResponse<SellerDashboardDto>?> GetDashboardAsync()
    {
        return await apiClient.GetAsync<ApiResponse<SellerDashboardDto>>("api/dashboard/seller");
    }
}
