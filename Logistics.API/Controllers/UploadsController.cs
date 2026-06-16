using System.Security.Claims;
using Logistics.Application.Common;
using Logistics.Application.Interfaces;
using Logistics.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Logistics.API.Controllers;

[ApiController]
[Route("api/uploads")]
public class UploadsController(
    IApplicationDbContext context,
    ICloudinaryService cloudinaryService) : ControllerBase
{
    [HttpPost("avatar")]
    [Authorize(Roles = "Customer,Seller,Shipper,Admin")]
    public async Task<IActionResult> UploadAvatar([FromForm] IFormFile file, CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUser(out var currentUserId, out _))
        {
            return Unauthorized();
        }

        var user = await context.Users
            .FirstOrDefaultAsync(user => user.Id == currentUserId, cancellationToken);

        if (user is null)
        {
            return NotFound(ApiResponse<string>.Fail("Không tìm thấy người dùng."));
        }

        try
        {
            var avatarUrl = await cloudinaryService.UploadAvatarAsync(file, currentUserId, cancellationToken);

            user.AvatarUrl = avatarUrl;
            user.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync(cancellationToken);

            return Ok(ApiResponse<string>.Ok(avatarUrl, "Tải ảnh đại diện thành công."));
        }
        catch (ArgumentException exception)
        {
            return BadRequest(ApiResponse<string>.Fail(exception.Message));
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            return BadRequest(ApiResponse<string>.Fail("Có lỗi xảy ra khi tải ảnh. Vui lòng thử lại."));
        }
    }

    [HttpPost("products/{productId:int}/image")]
    [Authorize(Roles = "Seller,Admin")]
    public async Task<IActionResult> UploadProductImage(
        int productId,
        [FromForm] IFormFile file,
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUser(out var currentUserId, out var currentUserRole))
        {
            return Unauthorized();
        }

        var product = await context.Products
            .FirstOrDefaultAsync(product => product.Id == productId, cancellationToken);

        if (product is null)
        {
            return NotFound(ApiResponse<string>.Fail("Không tìm thấy sản phẩm."));
        }

        if (currentUserRole != UserRole.Admin && product.SellerId != currentUserId)
        {
            return BadRequest(ApiResponse<string>.Fail("Không có quyền cập nhật ảnh sản phẩm này."));
        }

        try
        {
            var imageUrl = await cloudinaryService.UploadProductImageAsync(file, productId, cancellationToken);

            product.ImageUrl = imageUrl;
            product.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync(cancellationToken);

            return Ok(ApiResponse<string>.Ok(imageUrl, "Tải ảnh sản phẩm thành công."));
        }
        catch (ArgumentException exception)
        {
            return BadRequest(ApiResponse<string>.Fail(exception.Message));
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            return BadRequest(ApiResponse<string>.Fail("Có lỗi xảy ra khi tải ảnh. Vui lòng thử lại."));
        }
    }

    private bool TryGetCurrentUser(out int userId, out UserRole role)
    {
        userId = 0;
        role = default;

        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var roleClaim = User.FindFirstValue(ClaimTypes.Role);

        return int.TryParse(userIdClaim, out userId)
            && Enum.TryParse(roleClaim, out role);
    }
}
