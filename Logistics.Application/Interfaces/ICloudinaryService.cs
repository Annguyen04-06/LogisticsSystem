using Microsoft.AspNetCore.Http;

namespace Logistics.Application.Interfaces;

public interface ICloudinaryService
{
    Task<string> UploadAvatarAsync(IFormFile file, int userId, CancellationToken cancellationToken);
    Task<string> UploadProductImageAsync(IFormFile file, int productId, CancellationToken cancellationToken);
}
