using System.Net.Http.Json;
using System.Globalization;
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

    public async Task<ApiResponse<BankingQrDto>?> GetBankingQrAsync(decimal amount)
    {
        var amountText = amount.ToString(CultureInfo.InvariantCulture);
        return await apiClient.GetAsync<ApiResponse<BankingQrDto>>($"api/payments/banking-qr?amount={amountText}");
    }

    public async Task<ApiResponse<BankingQrDto>?> GetOrderBankingQrAsync(int orderId)
    {
        return await apiClient.GetAsync<ApiResponse<BankingQrDto>>($"api/payments/orders/{orderId}/banking-qr");
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
