using Logistics.Application.Interfaces;
using Logistics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Logistics.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IApplicationDbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderDetail> OrderDetails => Set<OrderDetail>();
    public DbSet<Delivery> Deliveries => Set<Delivery>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Wallet> Wallets => Set<Wallet>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<PaymentTransaction> PaymentTransactions => Set<PaymentTransaction>();
    public DbSet<Coupon> Coupons => Set<Coupon>();
    public DbSet<CouponUsage> CouponUsages => Set<CouponUsage>();
    public DbSet<SupportTicket> SupportTickets => Set<SupportTicket>();
    public DbSet<TicketReply> TicketReplies => Set<TicketReply>();
    public DbSet<SellerRating> SellerRatings => Set<SellerRating>();
    public DbSet<StockTransaction> StockTransactions => Set<StockTransaction>();
    public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken)
    {
        return Database.BeginTransactionAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(user => user.Email)
            .IsUnique();

        modelBuilder.Entity<Coupon>()
            .HasIndex(coupon => coupon.Code)
            .IsUnique();

        modelBuilder.Entity<Wallet>()
            .HasIndex(wallet => wallet.UserId)
            .IsUnique();

        ConfigureMoneyPrecision(modelBuilder);
        ConfigureRelationships(modelBuilder);
    }

    private static void ConfigureMoneyPrecision(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().Property(product => product.Price).HasPrecision(18, 2);

        modelBuilder.Entity<Order>().Property(order => order.TotalAmount).HasPrecision(18, 2);
        modelBuilder.Entity<Order>().Property(order => order.DiscountAmount).HasPrecision(18, 2);
        modelBuilder.Entity<Order>().Property(order => order.FinalAmount).HasPrecision(18, 2);

        modelBuilder.Entity<OrderDetail>().Property(detail => detail.UnitPrice).HasPrecision(18, 2);
        modelBuilder.Entity<OrderDetail>().Property(detail => detail.TotalPrice).HasPrecision(18, 2);

        modelBuilder.Entity<Wallet>().Property(wallet => wallet.Balance).HasPrecision(18, 2);
        modelBuilder.Entity<Payment>().Property(payment => payment.Amount).HasPrecision(18, 2);
        modelBuilder.Entity<PaymentTransaction>().Property(transaction => transaction.Amount).HasPrecision(18, 2);

        modelBuilder.Entity<Coupon>().Property(coupon => coupon.DiscountValue).HasPrecision(18, 2);
        modelBuilder.Entity<Coupon>().Property(coupon => coupon.MinOrderAmount).HasPrecision(18, 2);
        modelBuilder.Entity<CouponUsage>().Property(usage => usage.DiscountAmount).HasPrecision(18, 2);
    }

    private static void ConfigureRelationships(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>()
            .HasOne<Category>()
            .WithMany()
            .HasForeignKey(product => product.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Product>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(product => product.SellerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Cart>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(cart => cart.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CartItem>()
            .HasOne<Cart>()
            .WithMany()
            .HasForeignKey(item => item.CartId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CartItem>()
            .HasOne<Product>()
            .WithMany()
            .HasForeignKey(item => item.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Order>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(order => order.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Order>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(order => order.SellerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Order>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(order => order.ShipperId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<OrderDetail>()
            .HasOne<Order>()
            .WithMany()
            .HasForeignKey(detail => detail.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<OrderDetail>()
            .HasOne<Product>()
            .WithMany()
            .HasForeignKey(detail => detail.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Delivery>()
            .HasOne<Order>()
            .WithMany()
            .HasForeignKey(delivery => delivery.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Delivery>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(delivery => delivery.ShipperId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Review>()
            .HasOne<Product>()
            .WithMany()
            .HasForeignKey(review => review.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Review>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(review => review.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Review>()
            .HasOne<Order>()
            .WithMany()
            .HasForeignKey(review => review.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Wallet>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(wallet => wallet.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Payment>()
            .HasOne<Order>()
            .WithMany()
            .HasForeignKey(payment => payment.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Payment>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(payment => payment.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PaymentTransaction>()
            .HasOne<Payment>()
            .WithMany()
            .HasForeignKey(transaction => transaction.PaymentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CouponUsage>()
            .HasOne<Coupon>()
            .WithMany()
            .HasForeignKey(usage => usage.CouponId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CouponUsage>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(usage => usage.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CouponUsage>()
            .HasOne<Order>()
            .WithMany()
            .HasForeignKey(usage => usage.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SupportTicket>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(ticket => ticket.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SupportTicket>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(ticket => ticket.SellerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SupportTicket>()
            .HasOne<Order>()
            .WithMany()
            .HasForeignKey(ticket => ticket.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TicketReply>()
            .HasOne<SupportTicket>()
            .WithMany()
            .HasForeignKey(reply => reply.SupportTicketId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TicketReply>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(reply => reply.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SellerRating>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(rating => rating.SellerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SellerRating>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(rating => rating.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SellerRating>()
            .HasOne<Order>()
            .WithMany()
            .HasForeignKey(rating => rating.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<StockTransaction>()
            .HasOne<Product>()
            .WithMany()
            .HasForeignKey(transaction => transaction.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PasswordResetToken>()
            .HasOne(token => token.User)
            .WithMany()
            .HasForeignKey(token => token.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
