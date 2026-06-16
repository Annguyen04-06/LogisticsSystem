using System.Net.Http.Json;
using Logistics.Web.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace Logistics.Web.Services;

public class UploadApiService(ApiClientService apiClient)
{
    private const long AvatarMaxSize = 2 * 1024 * 1024;
    private const long ProductImageMaxSize = 3 * 1024 * 1024;

    public async Task<ApiResponse<string>?> UploadAvatarAsync(IBrowserFile file)
    {
        using var content = CreateFileContent(file, AvatarMaxSize);
        var response = await apiClient.PostMultipartAsync("api/uploads/avatar", content);
        return await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
    }

    public async Task<ApiResponse<string>?> UploadProductImageAsync(int productId, IBrowserFile file)
    {
        using var content = CreateFileContent(file, ProductImageMaxSize);
        var response = await apiClient.PostMultipartAsync($"api/uploads/products/{productId}/image", content);
        return await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
    }

    private static MultipartFormDataContent CreateFileContent(IBrowserFile file, long maxSize)
    {
        var content = new MultipartFormDataContent();
        var fileContent = new StreamContent(file.OpenReadStream(maxSize));
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
        content.Add(fileContent, "file", file.Name);
        return content;
    }
}
