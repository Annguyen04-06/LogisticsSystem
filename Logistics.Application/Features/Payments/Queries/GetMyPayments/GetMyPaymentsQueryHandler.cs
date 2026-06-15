using Logistics.Application.Common;
using Logistics.Application.DTOs.Payments;
using Logistics.Application.Interfaces;
using Logistics.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Payments.Queries.GetMyPayments;

public class GetMyPaymentsQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetMyPaymentsQuery, ApiResponse<List<PaymentDto>>>
{
    public async Task<ApiResponse<List<PaymentDto>>> Handle(GetMyPaymentsQuery request, CancellationToken cancellationToken)
    {
        if (request.CurrentUserRole != UserRole.Customer)
        {
            return ApiResponse<List<PaymentDto>>.Fail("Only customer can view my payments.");
        }

        var payments = await context.Payments
            .Where(payment => payment.UserId == request.CurrentUserId)
            .OrderByDescending(payment => payment.CreatedAt)
            .Select(payment => PaymentDtoMapper.ToPaymentDto(payment))
            .ToListAsync(cancellationToken);

        return ApiResponse<List<PaymentDto>>.Ok(payments);
    }
}
