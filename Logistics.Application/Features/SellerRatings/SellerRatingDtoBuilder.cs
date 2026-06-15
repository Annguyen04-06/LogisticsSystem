using Logistics.Application.DTOs.SellerRatings;
using Logistics.Application.Interfaces;
using Logistics.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.SellerRatings;

internal static class SellerRatingDtoBuilder
{
    public static async Task<List<SellerRatingDto>> BuildListAsync(
        IApplicationDbContext context,
        IQueryable<SellerRating> ratingsQuery,
        CancellationToken cancellationToken)
    {
        return await (
            from rating in ratingsQuery
            join seller in context.Users on rating.SellerId equals seller.Id
            join customer in context.Users on rating.CustomerId equals customer.Id
            orderby rating.CreatedAt descending
            select new SellerRatingDto
            {
                Id = rating.Id,
                SellerId = rating.SellerId,
                SellerName = seller.FullName,
                CustomerId = rating.CustomerId,
                CustomerName = customer.FullName,
                OrderId = rating.OrderId,
                IsLike = rating.IsLike,
                Comment = rating.Comment,
                CreatedAt = rating.CreatedAt
            }).ToListAsync(cancellationToken);
    }

    public static async Task<SellerRatingDto?> BuildOneAsync(
        IApplicationDbContext context,
        IQueryable<SellerRating> ratingsQuery,
        CancellationToken cancellationToken)
    {
        var ratings = await BuildListAsync(context, ratingsQuery, cancellationToken);
        return ratings.FirstOrDefault();
    }
}
