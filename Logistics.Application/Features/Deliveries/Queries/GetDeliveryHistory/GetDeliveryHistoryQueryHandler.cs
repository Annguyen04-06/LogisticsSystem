using Logistics.Application.Common;
using Logistics.Application.DTOs.Deliveries;
using Logistics.Application.Interfaces;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Deliveries.Queries.GetDeliveryHistory;

public class GetDeliveryHistoryQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetDeliveryHistoryQuery, ApiResponse<List<DeliveryDto>>>
{
    public async Task<ApiResponse<List<DeliveryDto>>> Handle(
        GetDeliveryHistoryQuery request,
        CancellationToken cancellationToken)
    {
        if (request.CurrentUserRole is not UserRole.Shipper and not UserRole.Admin)
        {
            return ApiResponse<List<DeliveryDto>>.Fail("Only shipper or admin can view delivery history.");
        }

        var deliveriesQuery = context.Deliveries
            .Where(delivery => delivery.Status == OrderStatus.Delivered.ToString());

        if (request.CurrentUserRole == UserRole.Shipper)
        {
            deliveriesQuery = deliveriesQuery.Where(delivery => delivery.ShipperId == request.CurrentUserId);
        }

        var deliveries = await DeliveryDtoBuilder.BuildListAsync(context, deliveriesQuery, cancellationToken);
        return ApiResponse<List<DeliveryDto>>.Ok(deliveries);
    }
}
