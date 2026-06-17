using Logistics.Application.DTOs.Tickets;
using Logistics.Application.Interfaces;
using Logistics.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Tickets;

internal static class TicketDtoBuilder
{
    public static async Task<List<SupportTicketDto>> BuildListAsync(
        IApplicationDbContext context,
        IQueryable<SupportTicket> ticketsQuery,
        CancellationToken cancellationToken)
    {
        var tickets = await (
            from ticket in ticketsQuery
            join customer in context.Users on ticket.CustomerId equals customer.Id
            join seller in context.Users on ticket.SellerId equals seller.Id into sellerGroup
            from seller in sellerGroup.DefaultIfEmpty()
            orderby ticket.CreatedAt descending
            select new SupportTicketDto
            {
                Id = ticket.Id,
                CustomerId = ticket.CustomerId,
                CustomerName = customer.FullName,
                SellerId = ticket.SellerId,
                SellerName = seller == null ? null : seller.FullName,
                OrderId = ticket.OrderId,
                Title = ticket.Title,
                Content = ticket.Content,
                Status = ticket.Status,
                CreatedAt = ticket.CreatedAt
            }).ToListAsync(cancellationToken);

        if (tickets.Count == 0)
        {
            return tickets;
        }

        var ticketIds = tickets.Select(ticket => ticket.Id).ToList();
        var replies = await (
            from reply in context.TicketReplies
            join user in context.Users on reply.UserId equals user.Id
            where ticketIds.Contains(reply.SupportTicketId)
            orderby reply.CreatedAt
            select new
            {
                reply.SupportTicketId,
                Reply = new TicketReplyDto
                {
                    Id = reply.Id,
                    TicketId = reply.SupportTicketId,
                    UserId = reply.UserId,
                    UserName = user.FullName,
                    Message = reply.Content,
                    CreatedAt = reply.CreatedAt
                }
            }).ToListAsync(cancellationToken);

        var repliesByTicket = replies
            .GroupBy(reply => reply.SupportTicketId)
            .ToDictionary(group => group.Key, group => group.Select(item => item.Reply).ToList());

        foreach (var ticket in tickets)
        {
            if (repliesByTicket.TryGetValue(ticket.Id, out var ticketReplies))
            {
                ticket.Replies = ticketReplies;
            }
        }

        return tickets;
    }

    public static async Task<SupportTicketDto?> BuildOneAsync(
        IApplicationDbContext context,
        IQueryable<SupportTicket> ticketsQuery,
        CancellationToken cancellationToken)
    {
        var tickets = await BuildListAsync(context, ticketsQuery, cancellationToken);
        return tickets.FirstOrDefault();
    }
}
