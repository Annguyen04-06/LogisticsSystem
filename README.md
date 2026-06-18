# LogisticsSystem

## Giới thiệu dự án

LogisticsSystem là hệ thống quản lý bán hàng và logistics gồm REST API ASP.NET Core và giao diện Blazor. Dự án hỗ trợ quản lý sản phẩm, giỏ hàng, đơn hàng, thanh toán, giao hàng, người dùng, báo cáo và yêu cầu hỗ trợ theo từng vai trò.

## Công nghệ sử dụng

- .NET 10, ASP.NET Core Web API và Blazor Server
- Entity Framework Core 10 và SQL Server
- MediatR, FluentValidation và JWT Bearer Authentication
- Swagger/OpenAPI
- ClosedXML và QuestPDF cho báo cáo
- Cloudinary cho lưu trữ hình ảnh và MailKit cho email

## Chức năng theo vai trò

- Admin: quản lý người dùng, danh mục, mã giảm giá, báo cáo và bảng điều khiển tổng quan.
- Customer: xem sản phẩm, quản lý giỏ hàng, đặt/hủy đơn, thanh toán, ví, hồ sơ và yêu cầu hỗ trợ.
- Seller: quản lý sản phẩm, đơn bán hàng, doanh thu và bảng điều khiển người bán.
- Shipper: xem đơn được phân công, cập nhật trạng thái giao hàng và lịch sử giao hàng.

## Cấu hình database

Ứng dụng sử dụng SQL Server. Cấu hình chuỗi kết nối `ConnectionStrings:DefaultConnection` bằng biến môi trường hoặc cấu hình cục bộ, dựa trên file mẫu `Logistics.API/appsettings.Example.json`.

Ví dụ trong PowerShell:

```powershell
$env:ConnectionStrings__DefaultConnection="Server=localhost;Database=LogisticsSystemDb;Trusted_Connection=True;TrustServerCertificate=True"
```

Không commit mật khẩu database, JWT key, SMTP password hoặc API secret lên repository.

## Cách chạy API

```powershell
dotnet restore
dotnet run --project Logistics.API
```

API mặc định chạy tại `http://localhost:5203`; Swagger khả dụng tại `http://localhost:5203/swagger` trong môi trường Development. Khi API khởi động, dữ liệu và tài khoản demo được khởi tạo tự động.

## Cách chạy Web

Mở một cửa sổ terminal khác sau khi API đã chạy:

```powershell
dotnet run --project Logistics.Web
```

Web mặc định chạy tại `http://localhost:5039` và gọi API tại `http://localhost:5203`.

Hướng dẫn chi tiết: [HUONG_DAN_CHAY.md](HUONG_DAN_CHAY.md).

## Tài khoản demo

- Admin: `admin@gmail.com` / `Annguyen@123`
- Customer: `customer@gmail.com` / `Annguyen@123`
- Seller: `seller@gmail.com` / `Annguyen@123`
- Shipper: `shipper@gmail.com` / `Annguyen@123`

Các tài khoản demo mở rộng (`customer2@gmail.com` đến `customer4@gmail.com`, `seller2@gmail.com` đến `seller4@gmail.com`, `shipper2@gmail.com`, `shipper3@gmail.com`) cũng sử dụng mật khẩu `Annguyen@123`.
