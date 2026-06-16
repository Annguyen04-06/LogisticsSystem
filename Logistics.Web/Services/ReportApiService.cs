using Logistics.Web.Models;

namespace Logistics.Web.Services;

public class ReportApiService(ApiClientService apiClient)
{
    public async Task<ApiResponse<RevenueReportDto>?> GetAdminRevenueAsync()
    {
        return await apiClient.GetAsync<ApiResponse<RevenueReportDto>>("api/reports/admin/revenue");
    }

    public async Task<ApiResponse<List<TopProductReportDto>>?> GetTopProductsAsync()
    {
        return await apiClient.GetAsync<ApiResponse<List<TopProductReportDto>>>("api/reports/admin/top-products");
    }

    public async Task<ApiResponse<List<TopSellerReportDto>>?> GetTopSellersAsync()
    {
        return await apiClient.GetAsync<ApiResponse<List<TopSellerReportDto>>>("api/reports/admin/top-sellers");
    }

    public async Task<ApiResponse<List<OrderStatusStatisticsDto>>?> GetOrderStatusStatisticsAsync()
    {
        return await apiClient.GetAsync<ApiResponse<List<OrderStatusStatisticsDto>>>("api/reports/admin/order-status-statistics");
    }

    public async Task<ApiResponse<List<OrderStatisticsByMonthDto>>?> GetOrdersByMonthAsync()
    {
        return await apiClient.GetAsync<ApiResponse<List<OrderStatisticsByMonthDto>>>("api/dashboard/orders-by-month");
    }

    public async Task<ApiResponse<SellerRevenueReportDto>?> GetSellerRevenueAsync()
    {
        return await apiClient.GetAsync<ApiResponse<SellerRevenueReportDto>>("api/reports/seller/revenue");
    }

    public async Task<byte[]> DownloadOrdersExcelAsync()
    {
        return await apiClient.GetBytesAsync("api/reports/admin/orders-excel");
    }

    public async Task<byte[]> DownloadAdminRevenueExcelAsync()
    {
        return await apiClient.GetBytesAsync("api/reports/admin/revenue-excel");
    }

    public async Task<byte[]> DownloadSellerRevenueExcelAsync()
    {
        return await apiClient.GetBytesAsync("api/reports/seller/revenue-excel");
    }

    public async Task<byte[]> DownloadInvoicePdfAsync(int orderId)
    {
        return await apiClient.GetBytesAsync($"api/reports/orders/{orderId}/invoice-pdf");
    }
}
