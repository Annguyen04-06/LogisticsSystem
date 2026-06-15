using Logistics.Application.DTOs.Reviews;
using Logistics.Application.Interfaces;
using Logistics.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Features.Reviews;

internal static class ReviewDtoBuilder
{
    public static async Task<List<ReviewDto>> BuildListAsync(
        IApplicationDbContext context,
        IQueryable<Review> reviewsQuery,
        CancellationToken cancellationToken)
    {
        return await (
            from review in reviewsQuery
            join product in context.Products on review.ProductId equals product.Id
            join customer in context.Users on review.CustomerId equals customer.Id
            orderby review.CreatedAt descending
            select new ReviewDto
            {
                Id = review.Id,
                ProductId = review.ProductId,
                ProductName = product.Name,
                CustomerId = review.CustomerId,
                CustomerName = customer.FullName,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt
            }).ToListAsync(cancellationToken);
    }

    public static async Task<ReviewDto?> BuildOneAsync(
        IApplicationDbContext context,
        IQueryable<Review> reviewsQuery,
        CancellationToken cancellationToken)
    {
        var reviews = await BuildListAsync(context, reviewsQuery, cancellationToken);
        return reviews.FirstOrDefault();
    }
}
