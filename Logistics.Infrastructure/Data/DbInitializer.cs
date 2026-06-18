using Logistics.Application.Interfaces;
using Logistics.Domain.Entities;
using Logistics.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Logistics.Infrastructure.Data;

public static class DbInitializer
{
    private const string DemoPassword = "Annguyen@123";

    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var logger = scope.ServiceProvider
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("DbInitializer");
        var cloudinaryService = ResolveCloudinaryService(scope.ServiceProvider, logger);

        await context.Database.MigrateAsync();

        await SeedUsersAsync(context);
        await SeedCategoriesAsync(context);
        await SeedProductsAsync(context, cloudinaryService, logger);
        await SeedDemoOrdersAsync(context);
    }

    private static async Task SeedUsersAsync(AppDbContext context)
    {
        var users = new[]
        {
            new UserSeed("Admin", "admin@gmail.com", UserRole.Admin, "0900000001", "Admin Address"),
            new UserSeed("Customer", "customer@gmail.com", UserRole.Customer, "0900000002", "Customer Address"),
            new UserSeed("Seller", "seller@gmail.com", UserRole.Seller, "0900000003", "Seller Address"),
            new UserSeed("Shipper", "shipper@gmail.com", UserRole.Shipper, "0900000004", "Shipper Address"),
            new UserSeed("Nguyễn Minh Anh", "customer2@gmail.com", UserRole.Customer, "0900000005", "Đà Nẵng"),
            new UserSeed("Trần Hoàng Nam", "customer3@gmail.com", UserRole.Customer, "0900000006", "Hồ Chí Minh"),
            new UserSeed("Lê Thu Hà", "customer4@gmail.com", UserRole.Customer, "0900000007", "Hà Nội"),
            new UserSeed("Shop Công Nghệ Đà Nẵng", "seller2@gmail.com", UserRole.Seller, "0900000008", "Đà Nẵng"),
            new UserSeed("Fashion House", "seller3@gmail.com", UserRole.Seller, "0900000009", "Hồ Chí Minh"),
            new UserSeed("Nhà Sách IT", "seller4@gmail.com", UserRole.Seller, "0900000010", "Hà Nội"),
            new UserSeed("Nguyễn Văn Shipper 2", "shipper2@gmail.com", UserRole.Shipper, "0900000011", "Đà Nẵng"),
            new UserSeed("Trần Văn Shipper 3", "shipper3@gmail.com", UserRole.Shipper, "0900000012", "Hồ Chí Minh")
        };

        foreach (var seed in users)
        {
            var user = await context.Users.FirstOrDefaultAsync(user => user.Email == seed.Email);

            if (user is null)
            {
                user = new User
                {
                    FullName = seed.FullName,
                    Email = seed.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(DemoPassword),
                    PhoneNumber = seed.PhoneNumber,
                    Address = seed.Address,
                    Role = seed.Role,
                    IsApproved = true,
                    IsActive = true
                };

                context.Users.Add(user);
            }
            else
            {
                user.FullName = seed.FullName;
                if (!IsPasswordMatch(DemoPassword, user.PasswordHash))
                {
                    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(DemoPassword);
                }

                user.PhoneNumber = seed.PhoneNumber;
                user.Address = seed.Address;
                user.Role = seed.Role;
                user.IsApproved = true;
                user.IsActive = true;
            }
        }

        await context.SaveChangesAsync();

        var customerEmails = users
            .Where(user => user.Role == UserRole.Customer)
            .Select(user => user.Email)
            .ToList();

        var customers = await context.Users
            .Where(user => customerEmails.Contains(user.Email))
            .ToListAsync();

        foreach (var customer in customers)
        {
            var hasWallet = await context.Wallets.AnyAsync(wallet => wallet.UserId == customer.Id);

            if (!hasWallet)
            {
                context.Wallets.Add(new Wallet
                {
                    UserId = customer.Id,
                    Balance = 10000000
                });
            }
        }

        await context.SaveChangesAsync();
    }

    private static bool IsPasswordMatch(string password, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            return false;
        }

        try
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
        catch
        {
            return false;
        }
    }

    private static async Task SeedCategoriesAsync(AppDbContext context)
    {
        var categories = new[]
        {
            new CategorySeed("Electronics", "Thiết bị điện tử"),
            new CategorySeed("Fashion", "Thời trang"),
            new CategorySeed("Food", "Đồ ăn thức uống"),
            new CategorySeed("Books", "Sách"),
            new CategorySeed("Cosmetics", "Mỹ phẩm"),
            new CategorySeed("Home Appliances", "Đồ gia dụng"),
            new CategorySeed("Accessories", "Phụ kiện"),
            new CategorySeed("Sports", "Thể thao")
        };

        foreach (var seed in categories)
        {
            var category = await context.Categories.FirstOrDefaultAsync(category => category.Name == seed.Name);

            if (category is null)
            {
                context.Categories.Add(new Category
                {
                    Name = seed.Name,
                    Description = seed.Description,
                    IsActive = true
                });
            }
            else
            {
                category.Description = seed.Description;
                category.IsActive = true;
            }
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedProductsAsync(
        AppDbContext context,
        ICloudinaryService? cloudinaryService,
        ILogger logger)
    {
        var categories = await context.Categories.ToDictionaryAsync(category => category.Name);
        var sellers = await context.Users
            .Where(user => user.Role == UserRole.Seller)
            .ToDictionaryAsync(user => user.Email);

        var products = new[]
        {
            new ProductSeed("Laptop Gaming Acer", "Laptop gaming hiệu năng cao, phù hợp học tập và giải trí.", 22990000, 18, "Electronics", "seller2@gmail.com", "https://images.unsplash.com/photo-1603302576837-37561b2e2302?auto=format&fit=crop&w=900&q=80"),
            new ProductSeed("Chuột không dây Logitech", "Chuột không dây nhỏ gọn, pin bền và kết nối ổn định.", 450000, 80, "Electronics", "seller2@gmail.com", "https://images.unsplash.com/photo-1615663245857-ac93bb7c39e7?auto=format&fit=crop&w=900&q=80"),
            new ProductSeed("Tai nghe Bluetooth Sony", "Tai nghe không dây chống ồn, âm thanh rõ và pin lâu.", 1890000, 35, "Electronics", "seller2@gmail.com", "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?auto=format&fit=crop&w=900&q=80"),
            new ProductSeed("Bàn phím cơ RGB", "Bàn phím cơ LED RGB, gõ êm và phản hồi tốt.", 1250000, 42, "Electronics", "seller2@gmail.com", "https://images.unsplash.com/photo-1587829741301-dc798b83add3?auto=format&fit=crop&w=900&q=80"),
            new ProductSeed("Điện thoại Samsung Galaxy", "Điện thoại Android màn hình đẹp, camera sắc nét.", 12990000, 24, "Electronics", "seller2@gmail.com", "https://images.unsplash.com/photo-1598327105666-5b89351aff97?auto=format&fit=crop&w=900&q=80"),
            new ProductSeed("Laptop", "Laptop cho học tập và làm việc văn phòng.", 15000000, 20, "Electronics", "seller@gmail.com", "https://images.unsplash.com/photo-1496181133206-80ce9b88a853?auto=format&fit=crop&w=900&q=80"),
            new ProductSeed("Phone", "Điện thoại thông minh với đầy đủ tính năng hiện đại.", 8000000, 30, "Electronics", "seller@gmail.com", "https://images.unsplash.com/photo-1511707171634-5f897ff02aa9?auto=format&fit=crop&w=900&q=80"),

            new ProductSeed("Áo thun basic nam", "Áo thun cotton thoáng mát, dễ phối đồ hằng ngày.", 180000, 120, "Fashion", "seller3@gmail.com", "https://images.unsplash.com/photo-1521572163474-6864f9cf17ab?auto=format&fit=crop&w=900&q=80"),
            new ProductSeed("Áo khoác bomber", "Áo khoác bomber trẻ trung, chất vải bền đẹp.", 520000, 55, "Fashion", "seller3@gmail.com", "https://images.unsplash.com/photo-1542291026-7eec264c27ff?auto=format&fit=crop&w=900&q=80"),
            new ProductSeed("Giày sneaker trắng", "Sneaker trắng tối giản, phù hợp đi học và đi làm.", 690000, 65, "Fashion", "seller3@gmail.com", "https://images.unsplash.com/photo-1549298916-b41d501d3772?auto=format&fit=crop&w=900&q=80"),
            new ProductSeed("Túi đeo chéo thời trang", "Túi đeo chéo nhỏ gọn, phong cách năng động.", 320000, 70, "Fashion", "seller3@gmail.com", "https://images.unsplash.com/photo-1590874103328-eac38a683ce7?auto=format&fit=crop&w=900&q=80"),
            new ProductSeed("T-Shirt", "Áo thun cotton thoải mái, màu sắc dễ mặc.", 150000, 100, "Fashion", "seller@gmail.com", "https://images.unsplash.com/photo-1503341504253-dff4815485f1?auto=format&fit=crop&w=900&q=80"),

            new ProductSeed("Trà sữa truyền thống", "Trà sữa vị truyền thống thơm béo, topping dai ngon.", 35000, 150, "Food", "seller@gmail.com", "https://images.unsplash.com/photo-1558857563-b371033873b8?auto=format&fit=crop&w=900&q=80"),
            new ProductSeed("Cà phê rang xay Đà Nẵng", "Cà phê rang xay nguyên chất, vị đậm và hậu ngọt.", 145000, 90, "Food", "seller2@gmail.com", "https://images.unsplash.com/photo-1447933601403-0c6688de566e?auto=format&fit=crop&w=900&q=80"),
            new ProductSeed("Bánh quy bơ", "Bánh quy bơ giòn thơm, đóng hộp tiện lợi.", 85000, 110, "Food", "seller@gmail.com", "https://images.unsplash.com/photo-1499636136210-6f4ee915583e?auto=format&fit=crop&w=900&q=80"),
            new ProductSeed("Hạt điều rang muối", "Hạt điều rang muối giòn bùi, phù hợp làm quà.", 175000, 75, "Food", "seller@gmail.com", "https://images.unsplash.com/photo-1599599810769-bcde5a160d32?auto=format&fit=crop&w=900&q=80"),
            new ProductSeed("Milk Tea", "Trà sữa ngọt thơm, giao nhanh trong ngày.", 35000, 50, "Food", "seller@gmail.com", "https://images.unsplash.com/photo-1525385133512-2f3bdd039054?auto=format&fit=crop&w=900&q=80"),

            new ProductSeed("Sách Lập trình C# cơ bản", "Sách nhập môn C# dành cho sinh viên và người mới.", 210000, 60, "Books", "seller4@gmail.com", "https://images.unsplash.com/photo-1515879218367-8466d910aaa4?auto=format&fit=crop&w=900&q=80"),
            new ProductSeed("Sách ASP.NET Core Web API", "Hướng dẫn xây dựng REST API với ASP.NET Core.", 260000, 50, "Books", "seller4@gmail.com", "https://images.unsplash.com/photo-1532012197267-da84d127e765?auto=format&fit=crop&w=900&q=80"),
            new ProductSeed("Sách Clean Architecture", "Tư duy kiến trúc sạch cho ứng dụng .NET hiện đại.", 320000, 45, "Books", "seller4@gmail.com", "https://images.unsplash.com/photo-1519389950473-47ba0277781c?auto=format&fit=crop&w=900&q=80"),
            new ProductSeed("Sách JavaScript hiện đại", "Kiến thức JavaScript hiện đại cho phát triển web.", 240000, 55, "Books", "seller4@gmail.com", "https://images.unsplash.com/photo-1495446815901-a7297e633e8d?auto=format&fit=crop&w=900&q=80"),
            new ProductSeed("Programming Book", "Sách lập trình thân thiện cho người mới bắt đầu.", 250000, 40, "Books", "seller@gmail.com", "https://images.unsplash.com/photo-1516321318423-f06f85e504b3?auto=format&fit=crop&w=900&q=80"),

            new ProductSeed("Sữa rửa mặt dịu nhẹ", "Sữa rửa mặt lành tính, phù hợp da nhạy cảm.", 165000, 85, "Cosmetics", "seller3@gmail.com", "https://images.unsplash.com/photo-1556229010-6c3f2c9ca5f8?auto=format&fit=crop&w=900&q=80"),
            new ProductSeed("Kem chống nắng SPF50", "Kem chống nắng SPF50 mỏng nhẹ, không bết dính.", 285000, 75, "Cosmetics", "seller3@gmail.com", "https://images.unsplash.com/photo-1598440947619-2c35fc9aa908?auto=format&fit=crop&w=900&q=80"),
            new ProductSeed("Son dưỡng môi", "Son dưỡng giúp môi mềm mịn, dùng hằng ngày.", 95000, 130, "Cosmetics", "seller3@gmail.com", "https://images.unsplash.com/photo-1586495777744-4413f21062fa?auto=format&fit=crop&w=900&q=80"),

            new ProductSeed("Máy xay sinh tố", "Máy xay sinh tố công suất mạnh, dễ vệ sinh.", 890000, 32, "Home Appliances", "seller2@gmail.com", "https://images.unsplash.com/photo-1570222094114-d054a817e56b?auto=format&fit=crop&w=900&q=80"),
            new ProductSeed("Đèn bàn LED", "Đèn bàn LED chống mỏi mắt, nhiều mức sáng.", 360000, 70, "Home Appliances", "seller2@gmail.com", "https://images.unsplash.com/photo-1507473885765-e6ed057f782c?auto=format&fit=crop&w=900&q=80"),
            new ProductSeed("Bình giữ nhiệt", "Bình giữ nhiệt inox, giữ nóng lạnh nhiều giờ.", 220000, 95, "Home Appliances", "seller2@gmail.com", "https://images.unsplash.com/photo-1602143407151-7111542de6e8?auto=format&fit=crop&w=900&q=80"),

            new ProductSeed("Ốp lưng điện thoại", "Ốp lưng chống sốc, thiết kế tối giản.", 85000, 160, "Accessories", "seller2@gmail.com", "https://images.unsplash.com/photo-1601593346740-925612772716?auto=format&fit=crop&w=900&q=80"),
            new ProductSeed("Cáp sạc Type-C", "Cáp sạc Type-C bền, hỗ trợ sạc nhanh.", 120000, 140, "Accessories", "seller2@gmail.com", "https://images.unsplash.com/photo-1603539444875-76e7684265f6?auto=format&fit=crop&w=900&q=80"),
            new ProductSeed("Giá đỡ laptop", "Giá đỡ laptop gấp gọn, hỗ trợ tản nhiệt.", 250000, 88, "Accessories", "seller2@gmail.com", "https://images.unsplash.com/photo-1527443224154-c4a3942d3acf?auto=format&fit=crop&w=900&q=80")
        };

        foreach (var seed in products)
        {
            if (!categories.TryGetValue(seed.CategoryName, out var category)
                || !sellers.TryGetValue(seed.SellerEmail, out var seller))
            {
                continue;
            }

            var product = await context.Products
                .FirstOrDefaultAsync(product => product.Name == seed.Name && product.SellerId == seller.Id);

            if (product is null)
            {
                product = new Product
                {
                    Name = seed.Name,
                    Description = seed.Description,
                    Price = seed.Price,
                    Quantity = seed.Quantity,
                    CategoryId = category.Id,
                    SellerId = seller.Id,
                    ImageUrl = seed.ImageUrl,
                    IsActive = true
                };

                context.Products.Add(product);
                await context.SaveChangesAsync();
            }
            else
            {
                product.Description = seed.Description;
                product.CategoryId = category.Id;
                product.ImageUrl = string.IsNullOrWhiteSpace(product.ImageUrl) ? seed.ImageUrl : product.ImageUrl;
            }

            await UploadDemoProductImageIfNeededAsync(
                context,
                product,
                seed.ImageUrl,
                cloudinaryService,
                logger);
        }

        await context.SaveChangesAsync();
    }

    private static ICloudinaryService? ResolveCloudinaryService(IServiceProvider serviceProvider, ILogger logger)
    {
        try
        {
            return serviceProvider.GetService<ICloudinaryService>();
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Cloudinary demo image upload is disabled because configuration or service resolution failed.");
            return null;
        }
    }

    private static async Task UploadDemoProductImageIfNeededAsync(
        AppDbContext context,
        Product product,
        string sourceImageUrl,
        ICloudinaryService? cloudinaryService,
        ILogger logger)
    {
        if (cloudinaryService is null || IsCloudinaryUrl(product.ImageUrl))
        {
            return;
        }

        var imageUrl = string.IsNullOrWhiteSpace(product.ImageUrl)
            ? sourceImageUrl
            : product.ImageUrl;

        if (string.IsNullOrWhiteSpace(imageUrl) || IsCloudinaryUrl(imageUrl))
        {
            return;
        }

        try
        {
            var cloudinaryUrl = await cloudinaryService.UploadDemoProductImageAsync(
                imageUrl,
                product.Id,
                CancellationToken.None);

            if (!string.IsNullOrWhiteSpace(cloudinaryUrl))
            {
                product.ImageUrl = cloudinaryUrl;
                await context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(
                ex,
                "Could not upload demo product image to Cloudinary. ProductId: {ProductId}, ProductName: {ProductName}",
                product.Id,
                product.Name);
        }
    }

    private static bool IsCloudinaryUrl(string? imageUrl)
    {
        return !string.IsNullOrWhiteSpace(imageUrl)
            && imageUrl.Contains("res.cloudinary.com", StringComparison.OrdinalIgnoreCase);
    }

    private static async Task SeedDemoOrdersAsync(AppDbContext context)
    {
        var hasDemoOrders = await context.PaymentTransactions
            .AnyAsync(transaction => transaction.TransactionCode.StartsWith("DEMO-ORDER-"));

        if (hasDemoOrders)
        {
            return;
        }

        var users = await context.Users.ToDictionaryAsync(user => user.Email);
        var products = await context.Products
            .GroupBy(product => product.Name)
            .ToDictionaryAsync(group => group.Key, group => group.First());

        if (!users.TryGetValue("customer@gmail.com", out var customer)
            || !users.TryGetValue("customer2@gmail.com", out var customer2)
            || !users.TryGetValue("customer3@gmail.com", out var customer3)
            || !users.TryGetValue("shipper@gmail.com", out var shipper)
            || !users.TryGetValue("shipper2@gmail.com", out var shipper2)
            || !users.TryGetValue("shipper3@gmail.com", out var shipper3))
        {
            return;
        }

        var demoOrders = new[]
        {
            new DemoOrderSeed(customer.Id, shipper.Id, DateTime.UtcNow.AddDays(-18), "DEMO-ORDER-001", new[] { ("Laptop Gaming Acer", 1), ("Chuột không dây Logitech", 2) }),
            new DemoOrderSeed(customer2.Id, shipper2.Id, DateTime.UtcNow.AddDays(-14), "DEMO-ORDER-002", new[] { ("Áo thun basic nam", 3), ("Giày sneaker trắng", 1) }),
            new DemoOrderSeed(customer3.Id, shipper3.Id, DateTime.UtcNow.AddDays(-11), "DEMO-ORDER-003", new[] { ("Sách ASP.NET Core Web API", 1), ("Sách Clean Architecture", 1) }),
            new DemoOrderSeed(customer.Id, shipper2.Id, DateTime.UtcNow.AddDays(-7), "DEMO-ORDER-004", new[] { ("Tai nghe Bluetooth Sony", 1), ("Bàn phím cơ RGB", 1) }),
            new DemoOrderSeed(customer2.Id, shipper.Id, DateTime.UtcNow.AddDays(-4), "DEMO-ORDER-005", new[] { ("Kem chống nắng SPF50", 2), ("Son dưỡng môi", 2) }),
            new DemoOrderSeed(customer3.Id, shipper3.Id, DateTime.UtcNow.AddDays(-2), "DEMO-ORDER-006", new[] { ("Máy xay sinh tố", 1), ("Bình giữ nhiệt", 2) })
        };

        foreach (var seed in demoOrders)
        {
            var orderProducts = seed.Items
                .Select(item => products.TryGetValue(item.ProductName, out var product)
                    ? new DemoOrderItem(product, item.Quantity)
                    : null)
                .Where(item => item is not null)
                .Cast<DemoOrderItem>()
                .ToList();

            if (orderProducts.Count == 0)
            {
                continue;
            }

            var sellerIds = orderProducts.Select(item => item.Product.SellerId).Distinct().ToList();

            if (sellerIds.Count != 1)
            {
                continue;
            }

            var totalAmount = orderProducts.Sum(item => item.Product.Price * item.Quantity);
            var order = new Order
            {
                CustomerId = seed.CustomerId,
                SellerId = sellerIds[0],
                ShipperId = seed.ShipperId,
                TotalAmount = totalAmount,
                DiscountAmount = 0,
                FinalAmount = totalAmount,
                PaymentMethod = PaymentMethod.BankingDemo,
                Status = OrderStatus.Delivered,
                CreatedAt = seed.CreatedAt
            };

            context.Orders.Add(order);
            await context.SaveChangesAsync();

            foreach (var item in orderProducts)
            {
                context.OrderDetails.Add(new OrderDetail
                {
                    OrderId = order.Id,
                    ProductId = item.Product.Id,
                    Quantity = item.Quantity,
                    UnitPrice = item.Product.Price,
                    TotalPrice = item.Product.Price * item.Quantity,
                    CreatedAt = seed.CreatedAt
                });
            }

            context.Deliveries.Add(new Delivery
            {
                OrderId = order.Id,
                ShipperId = seed.ShipperId,
                Status = OrderStatus.Delivered.ToString(),
                AssignedAt = seed.CreatedAt.AddHours(2),
                DeliveredAt = seed.CreatedAt.AddDays(2),
                CreatedAt = seed.CreatedAt.AddHours(2)
            });

            var payment = new Payment
            {
                OrderId = order.Id,
                UserId = seed.CustomerId,
                Amount = totalAmount,
                Method = PaymentMethod.BankingDemo,
                Status = PaymentStatus.Paid,
                PaidAt = seed.CreatedAt,
                CreatedAt = seed.CreatedAt
            };

            context.Payments.Add(payment);
            await context.SaveChangesAsync();

            context.PaymentTransactions.Add(new PaymentTransaction
            {
                PaymentId = payment.Id,
                UserId = seed.CustomerId,
                OrderId = order.Id,
                TransactionCode = seed.TransactionCode,
                Amount = totalAmount,
                Status = PaymentStatus.Paid,
                Note = "Demo paid order for presentation reports",
                CreatedAt = seed.CreatedAt
            });

            await context.SaveChangesAsync();
        }
    }

    private sealed record UserSeed(string FullName, string Email, UserRole Role, string PhoneNumber, string Address);

    private sealed record CategorySeed(string Name, string Description);

    private sealed record ProductSeed(
        string Name,
        string Description,
        decimal Price,
        int Quantity,
        string CategoryName,
        string SellerEmail,
        string ImageUrl);

    private sealed record DemoOrderSeed(
        int CustomerId,
        int ShipperId,
        DateTime CreatedAt,
        string TransactionCode,
        IReadOnlyList<(string ProductName, int Quantity)> Items);

    private sealed record DemoOrderItem(Product Product, int Quantity);
}
