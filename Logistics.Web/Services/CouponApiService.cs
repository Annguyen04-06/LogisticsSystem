using System.Net.Http.Json;
using Logistics.Web.Models;

namespace Logistics.Web.Services;

public class CouponApiService(ApiClientService apiClient)
{
    public async Task<ApiResponse<List<CouponDto>>?> GetCouponsAsync()
    {
        return await apiClient.GetAsync<ApiResponse<List<CouponDto>>>("api/coupons");
    }

    public async Task<ApiResponse<CouponDto>?> CreateAsync(CreateCouponDto request)
    {
        var response = await apiClient.PostAsync("api/coupons", request);
        return await response.Content.ReadFromJsonAsync<ApiResponse<CouponDto>>();
    }

    public async Task<ApiResponse<CouponDto>?> UpdateAsync(int id, UpdateCouponDto request)
    {
        var response = await apiClient.PutAsync($"api/coupons/{id}", request);
        return await response.Content.ReadFromJsonAsync<ApiResponse<CouponDto>>();
    }

    public async Task<ApiResponse<bool>?> DeleteAsync(int id)
    {
        var response = await apiClient.DeleteAsync($"api/coupons/{id}");
        return await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
    }
}
