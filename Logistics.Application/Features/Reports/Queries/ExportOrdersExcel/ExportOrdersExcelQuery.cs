using Logistics.Application.Common;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Reports.Queries.ExportOrdersExcel;

public record ExportOrdersExcelQuery(UserRole CurrentUserRole) : IRequest<ApiResponse<byte[]>>;
