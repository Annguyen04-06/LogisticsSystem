using System.Net.Http.Json;
using Logistics.Web.Models;

namespace Logistics.Web.Services;

public class PaymentApiService(ApiClientService apiClient)
{
    public async Task<ApiResponse<WalletDto>?> GetMyWalletAsync()
    {
        return await apiClient.GetAsync<ApiResponse<WalletDto>>("api/payments/my-wallet");
    }

    public async Task<ApiResponse<WalletDto>?> DepositWalletAsync(DepositWalletDto request)
    {
        var response = await apiClient.PostAsync("api/payments/deposit-wallet", request);
        return await response.Content.ReadFromJsonAsync<ApiResponse<WalletDto>>();
    }

    public async Task<ApiResponse<List<PaymentDto>>?> GetMyPaymentsAsync()
    {
        return await apiClient.GetAsync<ApiResponse<List<PaymentDto>>>("api/payments/my-payments");
    }

    public async Task<ApiResponse<PaymentDto>?> PayOrderAsync(PayOrderDto request)
    {
        var response = await apiClient.PostAsync("api/payments/pay-order", request);
        return await response.Content.ReadFromJsonAsync<ApiResponse<PaymentDto>>();
    }
}
