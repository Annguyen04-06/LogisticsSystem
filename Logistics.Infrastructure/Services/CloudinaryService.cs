using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Logistics.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Logistics.Infrastructure.Services;

public class CloudinaryService : ICloudinaryService
{
    private const long AvatarMaxSize = 2 * 1024 * 1024;
    private const long ProductImageMaxSize = 3 * 1024 * 1024;
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg",
        ".jpeg",
        ".png",
        ".webp"
    };

    private readonly Cloudinary _cloudinary;

    public CloudinaryService(IConfiguration configuration)
    {
        var cloudName = configuration["Cloudinary:CloudName"];
        var apiKey = configuration["Cloudinary:ApiKey"];
        var apiSecret = configuration["Cloudinary:ApiSecret"];

        if (string.IsNullOrWhiteSpace(cloudName) ||
            string.IsNullOrWhiteSpace(apiKey) ||
            string.IsNullOrWhiteSpace(apiSecret))
        {
            throw new InvalidOperationException("Cloudinary configuration is missing.");
        }

        var account = new Account(cloudName, apiKey, apiSecret);
        _cloudinary = new Cloudinary(account)
        {
            Api =
            {
                Secure = true
            }
        };
    }

    public Task<string> UploadAvatarAsync(IFormFile file, int userId, CancellationToken cancellationToken)
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var publicId = $"logistics/avatars/avatar-user-{userId}-{timestamp}";

        return UploadImageAsync(file, AvatarMaxSize, publicId, cancellationToken);
    }

    public Task<string> UploadProductImageAsync(IFormFile file, int productId, CancellationToken cancellationToken)
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var publicId = $"logistics/products/product-{productId}-{timestamp}";

        return UploadImageAsync(file, ProductImageMaxSize, publicId, cancellationToken);
    }

    public async Task<string> UploadDemoProductImageAsync(
        string imageUrl,
        int productId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            throw new ArgumentException("Vui lòng chọn file ảnh.");
        }

        cancellationToken.ThrowIfCancellationRequested();

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(imageUrl),
            Folder = "logistics/demo-products",
            PublicId = $"demo-product-{productId}",
            Overwrite = true
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        if (uploadResult.Error is not null)
        {
            throw new InvalidOperationException(uploadResult.Error.Message);
        }

        return uploadResult.SecureUrl?.ToString() ?? string.Empty;
    }

    private async Task<string> UploadImageAsync(
        IFormFile file,
        long maxSize,
        string publicId,
        CancellationToken cancellationToken)
    {
        ValidateImage(file, maxSize);
        cancellationToken.ThrowIfCancellationRequested();

        await using var stream = file.OpenReadStream();
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            PublicId = publicId,
            Overwrite = true
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        if (uploadResult.Error is not null)
        {
            throw new InvalidOperationException(uploadResult.Error.Message);
        }

        return uploadResult.SecureUrl?.ToString() ?? string.Empty;
    }

    private static void ValidateImage(IFormFile file, long maxSize)
    {
        if (file is null || file.Length == 0)
        {
            throw new ArgumentException("Vui lòng chọn file ảnh.");
        }

        var extension = Path.GetExtension(file.FileName);

        if (!AllowedExtensions.Contains(extension))
        {
            throw new ArgumentException("Định dạng ảnh không hợp lệ.");
        }

        if (file.Length > maxSize)
        {
            throw new ArgumentException("Dung lượng ảnh vượt quá giới hạn cho phép.");
        }
    }
}
