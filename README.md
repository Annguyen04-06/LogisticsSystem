<div align="center">

# 🚚 HỆ THỐNG QUẢN LÝ LOGISTICS

### Nền tảng quản lý bán hàng, giao hàng, thanh toán và báo cáo doanh thu

![.NET](https://img.shields.io/badge/.NET-ASP.NET_Core-512BD4?style=for-the-badge\&logo=dotnet)
![Blazor](https://img.shields.io/badge/Blazor-Web_UI-512BD4?style=for-the-badge\&logo=blazor)
![SQL Server](https://img.shields.io/badge/SQL_Server-Database-CC2927?style=for-the-badge\&logo=microsoftsqlserver)
![Bootstrap](https://img.shields.io/badge/Bootstrap-5-7952B3?style=for-the-badge\&logo=bootstrap)
![Cloudinary](https://img.shields.io/badge/Cloudinary-Image_Upload-3448C5?style=for-the-badge\&logo=cloudinary)
![Chart.js](https://img.shields.io/badge/Chart.js-Reports-FF6384?style=for-the-badge\&logo=chartdotjs)
![Status](https://img.shields.io/badge/Status-Completed-success?style=for-the-badge)

</div>

---

## 👨‍💻 Tác giả

| Thông tin  | Nội dung                                                            |
| ---------- | ------------------------------------------------------------------- |
| Họ tên     | **Nguyễn An**                                                       |
| GitHub     | [Annguyen04-06](https://github.com/Annguyen04-06)                   |
| Repository | [LogisticsSystem](https://github.com/Annguyen04-06/LogisticsSystem) |
| Dự án      | Hệ thống quản lý Logistics                                          |
| Mục tiêu   | Đồ án ASP.NET Core Web API + Blazor Web                             |

---

## 📌 Giới thiệu

**Hệ thống quản lý Logistics** là một nền tảng mô phỏng quy trình thương mại điện tử kết hợp quản lý giao hàng.

Dự án hỗ trợ đầy đủ các vai trò gồm **Khách hàng**, **Người bán**, **Người giao hàng** và **Quản trị viên**.

Hệ thống cho phép khách hàng mua hàng, thanh toán và theo dõi đơn hàng; người bán quản lý sản phẩm, xác nhận đơn và gán người giao hàng; người giao hàng cập nhật quá trình giao hàng; quản trị viên quản lý toàn bộ dữ liệu, người dùng, danh mục, mã giảm giá và báo cáo doanh thu.

---

## 🛠️ Công nghệ sử dụng

| Nhóm            | Công nghệ                    |
| --------------- | ---------------------------- |
| Backend         | ASP.NET Core Web API         |
| Frontend        | Blazor Web                   |
| Database        | SQL Server                   |
| ORM             | Entity Framework Core        |
| Architecture    | Clean Architecture, CQRS     |
| Authentication  | JWT Authentication           |
| Upload ảnh      | Cloudinary                   |
| Email           | Gmail SMTP                   |
| QR thanh toán   | VietQR Demo                  |
| UI Framework    | Bootstrap 5, Bootstrap Icons |
| Chart           | Chart.js                     |
| Version Control | Git, GitHub                  |

---

## ✨ Chức năng chính

### 👤 Khách hàng

* Đăng ký, đăng nhập
* Quên mật khẩu qua Gmail
* Xem, tìm kiếm, lọc và sắp xếp sản phẩm
* Thêm sản phẩm vào giỏ hàng
* Thay đổi số lượng và tự động cập nhật tổng tiền
* Đặt hàng
* Thanh toán bằng ví điện tử
* Nạp ví bằng mã QR demo
* Thanh toán chuyển khoản giả lập
* Theo dõi trạng thái đơn hàng
* Hủy đơn và nhận hoàn tiền nếu đã thanh toán
* Xem lịch sử thanh toán
* Xem lịch sử giao dịch ví
* Gửi phiếu hỗ trợ
* Cập nhật hồ sơ, avatar và mật khẩu

### 🏪 Người bán

* Quản lý sản phẩm
* Thêm, sửa, xóa/ngừng bán sản phẩm
* Upload ảnh sản phẩm bằng Cloudinary
* Xác nhận đơn hàng hợp lệ
* Chỉ xác nhận đơn ví/chuyển khoản khi khách đã thanh toán
* Gán đơn hàng cho đúng tài khoản người giao hàng
* Theo dõi đơn hàng người bán
* Xem dashboard người bán
* Cập nhật hồ sơ cá nhân

### 🚚 Người giao hàng

* Xem danh sách đơn được gán
* Bắt đầu giao hàng
* Hoàn tất giao hàng
* Xem lịch sử giao hàng
* Cập nhật hồ sơ cá nhân

### 🛡️ Quản trị viên

* Quản lý người dùng
* Duyệt, khóa, mở khóa và xóa tài khoản
* Quản lý danh mục sản phẩm
* Quản lý mã giảm giá
* Theo dõi phiếu hỗ trợ
* Quản lý báo cáo doanh thu
* Xem top sản phẩm bán chạy
* Xem top người bán có doanh thu cao
* Xem biểu đồ thống kê
* Cập nhật hồ sơ cá nhân

---

## 🧱 Cấu trúc dự án

Dự án được tổ chức theo hướng **Clean Architecture**, tách riêng các tầng như API, giao diện, nghiệp vụ, domain và hạ tầng. Cách chia này giúp dự án dễ mở rộng, dễ bảo trì và dễ kiểm thử.

```txt
LogisticsSystem
│
├── LogisticsSystem.sln
├── README.md
├── HUONG_DAN_CHAY.md
│
├── Logistics.API
│   ├── Controllers
│   │   ├── AuthController.cs
│   │   ├── ProductsController.cs
│   │   ├── CategoriesController.cs
│   │   ├── OrdersController.cs
│   │   ├── PaymentsController.cs
│   │   ├── DeliveriesController.cs
│   │   ├── UsersController.cs
│   │   ├── ReportsController.cs
│   │   ├── UploadsController.cs
│   │   ├── ProfileController.cs
│   │   └── SupportTicketsController.cs
│   │
│   ├── Program.cs
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   └── appsettings.Example.json
│
├── Logistics.Web
│   ├── Components
│   │   ├── Layout
│   │   │   ├── MainLayout.razor
│   │   │   └── NavMenu.razor
│   │   │
│   │   └── Pages
│   │       ├── Auth
│   │       │   ├── Login.razor
│   │       │   ├── Register.razor
│   │       │   ├── ForgotPassword.razor
│   │       │   └── ResetPassword.razor
│   │       │
│   │       ├── Customer
│   │       │   ├── Products.razor
│   │       │   ├── Cart.razor
│   │       │   ├── MyOrders.razor
│   │       │   ├── Payments.razor
│   │       │   └── MyTickets.razor
│   │       │
│   │       ├── Seller
│   │       │   ├── SellerDashboard.razor
│   │       │   ├── SellerProducts.razor
│   │       │   └── SellerOrders.razor
│   │       │
│   │       ├── Shipper
│   │       │   ├── ShipperOrders.razor
│   │       │   └── DeliveryHistory.razor
│   │       │
│   │       ├── Admin
│   │       │   ├── AdminDashboard.razor
│   │       │   ├── AdminUsers.razor
│   │       │   ├── AdminCategories.razor
│   │       │   ├── AdminCoupons.razor
│   │       │   └── AdminReports.razor
│   │       │
│   │       └── Profile.razor
│   │
│   ├── Services
│   │   ├── AuthService.cs
│   │   ├── ProductApiService.cs
│   │   ├── CartApiService.cs
│   │   ├── OrderApiService.cs
│   │   ├── PaymentApiService.cs
│   │   ├── ProfileApiService.cs
│   │   ├── UploadApiService.cs
│   │   └── UserApiService.cs
│   │
│   ├── Models
│   ├── wwwroot
│   │   ├── css
│   │   │   └── site.css
│   │   └── js
│   │       └── chartInterop.js
│   │
│   └── Program.cs
│
├── Logistics.Application
│   ├── DTOs
│   │   ├── Auth
│   │   ├── Products
│   │   ├── Categories
│   │   ├── Orders
│   │   ├── Payments
│   │   ├── Users
│   │   ├── Reports
│   │   └── SupportTickets
│   │
│   ├── Features
│   │   ├── Auth
│   │   ├── Products
│   │   ├── Categories
│   │   ├── Orders
│   │   ├── Payments
│   │   ├── Deliveries
│   │   ├── Users
│   │   ├── Reports
│   │   └── SupportTickets
│   │
│   ├── Interfaces
│   │   ├── IApplicationDbContext.cs
│   │   ├── IEmailService.cs
│   │   ├── ICloudinaryService.cs
│   │   └── IDateTimeService.cs
│   │
│   └── Common
│
├── Logistics.Domain
│   ├── Entities
│   │   ├── User.cs
│   │   ├── Product.cs
│   │   ├── Category.cs
│   │   ├── Cart.cs
│   │   ├── CartItem.cs
│   │   ├── Order.cs
│   │   ├── OrderDetail.cs
│   │   ├── Payment.cs
│   │   ├── PaymentTransaction.cs
│   │   ├── Wallet.cs
│   │   ├── Delivery.cs
│   │   ├── Coupon.cs
│   │   ├── CouponUsage.cs
│   │   ├── SupportTicket.cs
│   │   ├── TicketReply.cs
│   │   ├── Review.cs
│   │   ├── SellerRating.cs
│   │   └── PasswordResetToken.cs
│   │
│   └── Enums
│       ├── UserRole.cs
│       ├── OrderStatus.cs
│       ├── PaymentStatus.cs
│       ├── PaymentMethod.cs
│       ├── DeliveryStatus.cs
│       └── TicketStatus.cs
│
└── Logistics.Infrastructure
    ├── Data
    │   ├── AppDbContext.cs
    │   ├── DbInitializer.cs
    │   └── Migrations
    │
    ├── Services
    │   ├── EmailService.cs
    │   ├── CloudinaryService.cs
    │   └── DateTimeService.cs
    │
    └── DependencyInjection.cs
```

### 📌 Ý nghĩa từng project

| Project                    | Vai trò                                                                                            |
| -------------------------- | -------------------------------------------------------------------------------------------------- |
| `Logistics.API`            | Tầng API, chứa controller, cấu hình JWT, Swagger, middleware và endpoint cho frontend gọi          |
| `Logistics.Web`            | Giao diện Blazor Web, chứa các trang theo vai trò Customer, Seller, Shipper, Admin                 |
| `Logistics.Application`    | Tầng nghiệp vụ, chứa DTO, interface, CQRS command/query và xử lý logic ứng dụng                    |
| `Logistics.Domain`         | Tầng domain, chứa entity, enum và các mô hình dữ liệu cốt lõi                                      |
| `Logistics.Infrastructure` | Tầng hạ tầng, chứa DbContext, migration, seed data và service bên ngoài như Gmail SMTP, Cloudinary |

### 🔁 Luồng xử lý tổng quát

```txt
Người dùng thao tác trên Blazor Web
        ↓
Logistics.Web gọi API bằng HttpClient
        ↓
Logistics.API nhận request qua Controller
        ↓
Controller gửi Command/Query sang Application
        ↓
Application xử lý nghiệp vụ
        ↓
Infrastructure thao tác database hoặc service ngoài
        ↓
API trả kết quả về Web
        ↓
Blazor Web cập nhật giao diện
```

### 🧩 Ví dụ luồng đặt hàng

```txt
Customer chọn sản phẩm
        ↓
Thêm vào giỏ hàng
        ↓
Tạo đơn hàng
        ↓
Thanh toán ví / QR demo / COD
        ↓
Seller xác nhận đơn hợp lệ
        ↓
Seller gán shipper
        ↓
Shipper giao hàng
        ↓
Admin xem báo cáo doanh thu
```

---

## 🔐 Tài khoản demo

> Mật khẩu demo đã đáp ứng điều kiện bảo mật: tối thiểu 8 ký tự, có chữ hoa, chữ thường, số và ký tự đặc biệt.

| Vai trò         | Email                                           | Mật khẩu     |
| --------------- | ----------------------------------------------- | ------------ |
| Quản trị viên   | [admin@gmail.com](mailto:admin@gmail.com)       | Annguyen@123 |
| Khách hàng      | [customer@gmail.com](mailto:customer@gmail.com) | Annguyen@123 |
| Người bán       | [seller@gmail.com](mailto:seller@gmail.com)     | Annguyen@123 |
| Người giao hàng | [shipper@gmail.com](mailto:shipper@gmail.com)   | Annguyen@123 |

---

## ⚙️ Yêu cầu cài đặt

Trước khi chạy dự án, cần cài đặt:

* .NET SDK
* SQL Server
* Visual Studio 2022 hoặc Visual Studio Code
* SQL Server Management Studio
* Git
* Trình duyệt web hiện đại

---

## 🚀 Hướng dẫn chạy dự án

### 1. Clone project

```bash
git clone https://github.com/Annguyen04-06/LogisticsSystem.git
cd LogisticsSystem
```

### 2. Restore package

```bash
dotnet restore
```

### 3. Build project

```bash
dotnet build
```

### 4. Cấu hình database

Cập nhật connection string trong file:

```txt
Logistics.API/appsettings.Development.json
```

Ví dụ cấu hình:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=127.0.0.1,1433;Database=LogisticsSystemDb;User Id=sa;Password=your-sql-password;TrustServerCertificate=True;Encrypt=True;"
  },
  "Jwt": {
    "Key": "your-long-jwt-secret-key",
    "Issuer": "Logistics.API",
    "Audience": "Logistics.Client",
    "ExpireMinutes": 60
  },
  "Smtp": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "EnableSsl": true,
    "UserName": "your-email@gmail.com",
    "Password": "your-gmail-app-password",
    "FromName": "Hệ thống quản lý Logistics"
  },
  "Cloudinary": {
    "CloudName": "your-cloud-name",
    "ApiKey": "your-api-key",
    "ApiSecret": "your-api-secret"
  },
  "BankingQr": {
    "DemoMode": true,
    "BankCode": "MB",
    "AccountNumber": "0000000000",
    "AccountName": "DEMO LOGISTICS SYSTEM",
    "DescriptionPrefix": "NAPVI"
  }
}
```

### 5. Cập nhật database

```bash
dotnet ef database update -p Logistics.Infrastructure -s Logistics.API
```

Nếu máy chưa có EF Tool:

```bash
dotnet tool install --global dotnet-ef
```

### 6. Chạy API

```bash
dotnet run --project Logistics.API
```

Đường dẫn API:

```txt
http://localhost:5203
```

Swagger:

```txt
http://localhost:5203/swagger
```

### 7. Chạy Web

Mở terminal thứ hai:

```bash
dotnet run --project Logistics.Web
```

Đường dẫn Web:

```txt
http://localhost:5039
```

> Nếu port khác, hãy xem đường dẫn hiển thị trong terminal khi chạy project.

---

## 🎬 Kịch bản demo nhanh

1. Đăng nhập **Admin** để xem dashboard.
2. Admin tạo danh mục sản phẩm.
3. Đăng nhập **Seller** để tạo sản phẩm và upload ảnh.
4. Đăng nhập **Customer** để tìm kiếm sản phẩm.
5. Customer thêm sản phẩm vào giỏ hàng.
6. Customer thay đổi số lượng, tổng tiền cập nhật tự động.
7. Customer đặt hàng.
8. Customer thanh toán bằng ví điện tử hoặc QR demo.
9. Seller xác nhận đơn hàng đã thanh toán.
10. Seller gán shipper.
11. Shipper nhận đơn và hoàn tất giao hàng.
12. Admin xem báo cáo doanh thu, top sản phẩm và top người bán.

---

## ✅ Trạng thái dự án

| Hạng mục                       | Trạng thái   |
| ------------------------------ | ------------ |
| Authentication & Authorization | ✅ Hoàn thành |
| Quản lý người dùng             | ✅ Hoàn thành |
| Quản lý danh mục               | ✅ Hoàn thành |
| Quản lý sản phẩm               | ✅ Hoàn thành |
| Upload ảnh Cloudinary          | ✅ Hoàn thành |
| Giỏ hàng & đặt hàng            | ✅ Hoàn thành |
| Thanh toán ví / QR demo        | ✅ Hoàn thành |
| Hoàn tiền khi hủy đơn          | ✅ Hoàn thành |
| Giao hàng                      | ✅ Hoàn thành |
| Báo cáo thống kê               | ✅ Hoàn thành |
| Hỗ trợ khách hàng              | ✅ Hoàn thành |
| Quên mật khẩu qua Gmail        | ✅ Hoàn thành |
| Giao diện hiện đại             | ✅ Hoàn thành |

---

## 🔒 Ghi chú bảo mật

* Không nên public Gmail App Password.
* Không nên public Cloudinary API Secret.
* Không nên public mật khẩu SQL Server thật.
* Không nên dùng số tài khoản ngân hàng thật cho QR demo.
* Nên dùng file `appsettings.Example.json` để hướng dẫn cấu hình.
* File `appsettings.Development.json` nên giữ ở local khi deploy hoặc public source.

---

## 📸 Gợi ý ảnh demo

Có thể thêm ảnh giao diện vào thư mục:

```txt
docs/images
```

Sau đó nhúng vào README:

```md
![Dashboard](docs/images/dashboard.png)
![Products](docs/images/products.png)
![Reports](docs/images/reports.png)
```

---

<div align="center">

### 🚚 Logistics Management System

**Made with ❤️ by Nguyễn An**

⭐ Nếu bạn thấy dự án hữu ích, hãy để lại một Star cho repository này!

</div>
