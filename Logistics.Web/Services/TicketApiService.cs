using System.Net.Http.Json;
using Logistics.Web.Models;

namespace Logistics.Web.Services;

public class TicketApiService(ApiClientService apiClient)
{
    public async Task<ApiResponse<List<SupportTicketDto>>?> GetMyTicketsAsync()
    {
        return await apiClient.GetAsync<ApiResponse<List<SupportTicketDto>>>("api/tickets/my-tickets");
    }

    public async Task<ApiResponse<List<SupportTicketDto>>?> GetSellerTicketsAsync()
    {
        return await apiClient.GetAsync<ApiResponse<List<SupportTicketDto>>>("api/tickets/seller-tickets");
    }

    public async Task<ApiResponse<List<SupportTicketDto>>?> GetAllTicketsAsync()
    {
        return await apiClient.GetAsync<ApiResponse<List<SupportTicketDto>>>("api/tickets");
    }

    public async Task<ApiResponse<SupportTicketDto>?> GetByIdAsync(int id)
    {
        return await apiClient.GetAsync<ApiResponse<SupportTicketDto>>($"api/tickets/{id}");
    }

    public async Task<ApiResponse<SupportTicketDto>?> CreateAsync(CreateTicketDto request)
    {
        var response = await apiClient.PostAsync("api/tickets", request);
        return await response.Content.ReadFromJsonAsync<ApiResponse<SupportTicketDto>>();
    }

    public async Task<ApiResponse<SupportTicketDto>?> ReplyAsync(int id, ReplyTicketDto request)
    {
        var response = await apiClient.PostAsync($"api/tickets/{id}/reply", request);
        return await response.Content.ReadFromJsonAsync<ApiResponse<SupportTicketDto>>();
    }

    public async Task<ApiResponse<SupportTicketDto>?> UpdateStatusAsync(int id, string status)
    {
        var response = await apiClient.PutAsync(
            $"api/tickets/{id}/status",
            new UpdateTicketStatusDto { Status = status });

        return await response.Content.ReadFromJsonAsync<ApiResponse<SupportTicketDto>>();
    }

    public async Task<ApiResponse<SupportTicketDto>?> CloseAsync(int id)
    {
        var response = await apiClient.PutAsync($"api/tickets/{id}/close", new { });
        return await response.Content.ReadFromJsonAsync<ApiResponse<SupportTicketDto>>();
    }
}
