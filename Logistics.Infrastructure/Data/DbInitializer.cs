using Logistics.Domain.Entities;
using Logistics.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logistics.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await context.Database.MigrateAsync();

        if (await context.Users.AnyAsync())
        {
            return;
        }

        var admin = CreateUser(
            "Admin",
            "admin@gmail.com",
            UserRole.Admin,
            "0900000001",
            "Admin Address");

        var customer = CreateUser(
            "Customer",
            "customer@gmail.com",
            UserRole.Customer,
            "0900000002",
            "Customer Address");

        var seller = CreateUser(
            "Seller",
            "seller@gmail.com",
            UserRole.Seller,
            "0900000003",
            "Seller Address");

        var shipper = CreateUser(
            "Shipper",
            "shipper@gmail.com",
            UserRole.Shipper,
            "0900000004",
            "Shipper Address");

        var electronics = new Category
        {
            Name = "Electronics",
            Description = "Electronic devices and accessories",
            IsActive = true
        };

        var fashion = new Category
        {
            Name = "Fashion",
            Description = "Clothing and fashion products",
            IsActive = true
        };

        var food = new Category
        {
            Name = "Food",
            Description = "Food and drinks",
            IsActive = true
        };

        var books = new Category
        {
            Name = "Books",
            Description = "Books and learning materials",
            IsActive = true
        };

        context.Users.AddRange(admin, customer, seller, shipper);
        context.Categories.AddRange(electronics, fashion, food, books);
        await context.SaveChangesAsync();

        var products = new List<Product>
        {
            new()
            {
                Name = "Laptop",
                Description = "Laptop for work and study",
                Price = 15000000,
                Quantity = 20,
                CategoryId = electronics.Id,
                SellerId = seller.Id,
                IsActive = true
            },
            new()
            {
                Name = "Phone",
                Description = "Smartphone with modern features",
                Price = 8000000,
                Quantity = 30,
                CategoryId = electronics.Id,
                SellerId = seller.Id,
                IsActive = true
            },
            new()
            {
                Name = "T-Shirt",
                Description = "Comfortable cotton t-shirt",
                Price = 150000,
                Quantity = 100,
                CategoryId = fashion.Id,
                SellerId = seller.Id,
                IsActive = true
            },
            new()
            {
                Name = "Milk Tea",
                Description = "Sweet milk tea drink",
                Price = 35000,
                Quantity = 50,
                CategoryId = food.Id,
                SellerId = seller.Id,
                IsActive = true
            },
            new()
            {
                Name = "Programming Book",
                Description = "Beginner-friendly programming book",
                Price = 250000,
                Quantity = 40,
                CategoryId = books.Id,
                SellerId = seller.Id,
                IsActive = true
            }
        };

        context.Products.AddRange(products);
        context.Wallets.Add(new Wallet
        {
            UserId = customer.Id,
            Balance = 10000000
        });

        await context.SaveChangesAsync();
    }

    private static User CreateUser(
        string fullName,
        string email,
        UserRole role,
        string phoneNumber,
        string address)
    {
        return new User
        {
            FullName = fullName,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
            PhoneNumber = phoneNumber,
            Address = address,
            Role = role,
            IsApproved = true,
            IsActive = true
        };
    }
}
