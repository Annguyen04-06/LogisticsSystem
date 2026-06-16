using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Logistics.Web.Services;

public class ApiClientService(HttpClient httpClient, AuthStateService authState)
{
    public async Task<T?> GetAsync<T>(string url, CancellationToken cancellationToken = default)
    {
        ApplyAuthorizationHeader();
        return await httpClient.GetFromJsonAsync<T>(url, cancellationToken);
    }

    public async Task<HttpResponseMessage> PostAsync<T>(string url, T body, CancellationToken cancellationToken = default)
    {
        ApplyAuthorizationHeader();
        return await httpClient.PostAsJsonAsync(url, body, cancellationToken);
    }

    public async Task<HttpResponseMessage> PutAsync<T>(string url, T body, CancellationToken cancellationToken = default)
    {
        ApplyAuthorizationHeader();
        return await httpClient.PutAsJsonAsync(url, body, cancellationToken);
    }

    public async Task<HttpResponseMessage> DeleteAsync(string url, CancellationToken cancellationToken = default)
    {
        ApplyAuthorizationHeader();
        return await httpClient.DeleteAsync(url, cancellationToken);
    }

    public async Task<HttpResponseMessage> PostMultipartAsync(
        string url,
        MultipartFormDataContent content,
        CancellationToken cancellationToken = default)
    {
        ApplyAuthorizationHeader();
        return await httpClient.PostAsync(url, content, cancellationToken);
    }

    public async Task<byte[]> GetBytesAsync(string url, CancellationToken cancellationToken = default)
    {
        ApplyAuthorizationHeader();
        var response = await httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsByteArrayAsync(cancellationToken);
    }

    private void ApplyAuthorizationHeader()
    {
        httpClient.DefaultRequestHeaders.Authorization = string.IsNullOrWhiteSpace(authState.Token)
            ? null
            : new AuthenticationHeaderValue("Bearer", authState.Token);
    }
}
