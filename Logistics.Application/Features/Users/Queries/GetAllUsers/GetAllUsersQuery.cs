using Logistics.Application.Common;
using Logistics.Application.DTOs.Users;
using MediatR;

namespace Logistics.Application.Features.Users.Queries.GetAllUsers;

public class GetAllUsersQuery : IRequest<ApiResponse<List<UserDto>>>;
