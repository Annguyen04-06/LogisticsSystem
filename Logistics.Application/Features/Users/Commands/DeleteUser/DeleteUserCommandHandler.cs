using Logistics.Application.Common;
using Logistics.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Users.Commands.DeleteUser;

public class DeleteUserCommandHandler(IApplicationDbContext context)
    : IRequestHandler<DeleteUserCommand, ApiResponse<bool>>
{
    private const string CannotHardDeleteMessage = "Không thể xóa vĩnh viễn tài khoản này vì đã phát sinh dữ liệu trong hệ thống. Bạn có thể vô hiệu hóa tài khoản.";

    public async Task<ApiResponse<bool>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(user => user.Id == request.Id, cancellationToken);

        if (user is null)
        {
            return ApiResponse<bool>.Fail("Không tìm thấy người dùng.");
        }

        if (user.Id == request.CurrentUserId)
        {
            return ApiResponse<bool>.Fail("Quản trị viên không thể xóa chính mình.");
        }

        if (await HasImportantBusinessDataAsync(user.Id, cancellationToken))
        {
            return ApiResponse<bool>.Fail(CannotHardDeleteMessage);
        }

        await using var transaction = await context.BeginTransactionAsync(cancellationToken);

        try
        {
            await DeleteSafeRelatedDataAsync(user.Id, cancellationToken);

            context.Users.Remove(user);
            await context.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return ApiResponse<bool>.Ok(true, "Đã xóa vĩnh viễn tài khoản khỏi hệ thống.");
        }
        catch (DbUpdateException)
        {
            await transaction.RollbackAsync(cancellationToken);
            return ApiResponse<bool>.Fail(CannotHardDeleteMessage);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            await transaction.RollbackAsync(cancellationToken);
            return ApiResponse<bool>.Fail("Có lỗi xảy ra. Vui lòng thử lại.");
        }
    }

    private async Task<bool> HasImportantBusinessDataAsync(int userId, CancellationToken cancellationToken)
    {
        return await context.Orders.AnyAsync(order =>
                order.CustomerId == userId ||
                order.SellerId == userId ||
                order.ShipperId == userId,
                cancellationToken)
            || await context.Payments.AnyAsync(payment => payment.UserId == userId, cancellationToken)
            || await context.Deliveries.AnyAsync(delivery => delivery.ShipperId == userId, cancellationToken)
            || await context.Products.AnyAsync(product => product.SellerId == userId, cancellationToken)
            || await context.Reviews.AnyAsync(review => review.CustomerId == userId, cancellationToken)
            || await context.SellerRatings.AnyAsync(rating =>
                rating.SellerId == userId ||
                rating.CustomerId == userId,
                cancellationToken)
            || await context.SupportTickets.AnyAsync(ticket =>
                ticket.CustomerId == userId ||
                ticket.SellerId == userId,
                cancellationToken)
            || await context.TicketReplies.AnyAsync(reply => reply.UserId == userId, cancellationToken);
    }

    private async Task DeleteSafeRelatedDataAsync(int userId, CancellationToken cancellationToken)
    {
        var cartIds = await context.Carts
            .Where(cart => cart.UserId == userId)
            .Select(cart => cart.Id)
            .ToListAsync(cancellationToken);

        if (cartIds.Count > 0)
        {
            var cartItems = await context.CartItems
                .Where(item => cartIds.Contains(item.CartId))
                .ToListAsync(cancellationToken);

            context.CartItems.RemoveRange(cartItems);

            var carts = await context.Carts
                .Where(cart => cartIds.Contains(cart.Id))
                .ToListAsync(cancellationToken);

            context.Carts.RemoveRange(carts);
        }

        var wallets = await context.Wallets
            .Where(wallet => wallet.UserId == userId)
            .ToListAsync(cancellationToken);
        context.Wallets.RemoveRange(wallets);

        var passwordResetTokens = await context.PasswordResetTokens
            .Where(token => token.UserId == userId)
            .ToListAsync(cancellationToken);
        context.PasswordResetTokens.RemoveRange(passwordResetTokens);

        var couponUsages = await context.CouponUsages
            .Where(usage => usage.UserId == userId)
            .ToListAsync(cancellationToken);
        context.CouponUsages.RemoveRange(couponUsages);
    }
}
