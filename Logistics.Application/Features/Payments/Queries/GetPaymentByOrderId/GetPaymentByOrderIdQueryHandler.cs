using Logistics.Application.Common;
using Logistics.Application.DTOs.Payments;
using Logistics.Application.Interfaces;
using Logistics.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Payments.Queries.GetPaymentByOrderId;

public class GetPaymentByOrderIdQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetPaymentByOrderIdQuery, ApiResponse<PaymentDto>>
{
    public async Task<ApiResponse<PaymentDto>> Handle(GetPaymentByOrderIdQuery request, CancellationToken cancellationToken)
    {
        if (request.CurrentUserRole is not UserRole.Customer and not UserRole.Seller and not UserRole.Admin)
        {
            return ApiResponse<PaymentDto>.Fail("You do not have permission to view payment.");
        }

        var order = await context.Orders
            .FirstOrDefaultAsync(order => order.Id == request.OrderId, cancellationToken);

        if (order is null)
        {
            return ApiResponse<PaymentDto>.Fail("Order does not exist.");
        }

        var hasPermission = request.CurrentUserRole switch
        {
            UserRole.Admin => true,
            UserRole.Customer => order.CustomerId == request.CurrentUserId,
            UserRole.Seller => order.SellerId == request.CurrentUserId,
            _ => false
        };

        if (!hasPermission)
        {
            return ApiResponse<PaymentDto>.Fail("You do not have permission to view this payment.");
        }

        var payment = await context.Payments
            .Where(payment => payment.OrderId == order.Id)
            .OrderByDescending(payment => payment.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (payment is null)
        {
            return ApiResponse<PaymentDto>.Fail("Payment does not exist.");
        }

        return ApiResponse<PaymentDto>.Ok(PaymentDtoMapper.ToPaymentDto(payment));
    }
}
