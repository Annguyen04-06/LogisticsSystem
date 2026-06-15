using Logistics.Application.Common;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Reports.Queries.ExportOrderInvoicePdf;

public record ExportOrderInvoicePdfQuery(
    int OrderId,
    int CurrentUserId,
    UserRole CurrentUserRole) : IRequest<ApiResponse<byte[]>>;
