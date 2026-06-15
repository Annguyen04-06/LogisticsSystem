using Logistics.Application.Common;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Reports.Queries.ExportRevenueExcel;

public record ExportRevenueExcelQuery(
    int CurrentUserId,
    UserRole CurrentUserRole,
    int? SellerId = null) : IRequest<ApiResponse<byte[]>>;
