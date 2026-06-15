using Logistics.Application.Common;
using Logistics.Application.DTOs.Tickets;
using Logistics.Domain.Enums;
using MediatR;

namespace Logistics.Application.Features.Tickets.Commands.CreateTicket;

public class CreateTicketCommand : IRequest<ApiResponse<SupportTicketDto>>
{
    public CreateTicketDto Ticket { get; }
    public int CurrentUserId { get; }
    public UserRole CurrentUserRole { get; }

    public CreateTicketCommand(CreateTicketDto ticket, int currentUserId, UserRole currentUserRole)
    {
        Ticket = ticket;
        CurrentUserId = currentUserId;
        CurrentUserRole = currentUserRole;
    }
}
