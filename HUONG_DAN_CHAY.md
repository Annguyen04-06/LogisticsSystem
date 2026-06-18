# Hướng dẫn chạy LogisticsSystem

## Yêu cầu

- .NET 10 SDK
- SQL Server đang hoạt động
- PowerShell, Command Prompt hoặc terminal tương đương

## 1. Cấu hình

Tham khảo `Logistics.API/appsettings.Example.json` để biết các khóa cấu hình. Nên cung cấp dữ liệu nhạy cảm bằng biến môi trường.

Ví dụ PowerShell:

```powershell
$env:ConnectionStrings__DefaultConnection="Server=localhost;Database=LogisticsSystemDb;Trusted_Connection=True;TrustServerCertificate=True"
$env:Jwt__Key="replace-with-a-long-random-development-key"
```

Nếu sử dụng SMTP, Cloudinary hoặc mã QR ngân hàng, cấu hình thêm các biến môi trường tương ứng từ file mẫu. Không commit giá trị bí mật vào Git.

## 2. Khôi phục package và build

Tại thư mục gốc của repository:

```powershell
dotnet restore
dotnet build
```

Chỉ tiếp tục khi build thành công với 0 error.

## 3. Chạy API

```powershell
dotnet run --project Logistics.API
```

- API: `http://localhost:5203`
- Swagger: `http://localhost:5203/swagger`

API sẽ khởi tạo database và dữ liệu demo khi khởi động.

## 4. Chạy Web

Giữ API đang chạy, mở terminal khác tại thư mục gốc:

```powershell
dotnet run --project Logistics.Web
```

Mở `http://localhost:5039` trên trình duyệt.

## 5. Tài khoản demo

| Vai trò | Email | Mật khẩu |
| --- | --- | --- |
| Admin | `admin@gmail.com` | `Annguyen@123` |
| Customer | `customer@gmail.com` | `Annguyen@123` |
| Seller | `seller@gmail.com` | `Annguyen@123` |
| Shipper | `shipper@gmail.com` | `Annguyen@123` |

## Lỗi thường gặp

- Không kết nối được database: kiểm tra SQL Server và `ConnectionStrings__DefaultConnection`.
- Web không gọi được API: xác nhận API đang chạy tại `http://localhost:5203`.
- Lỗi chứng chỉ HTTPS cục bộ: dùng các địa chỉ HTTP ở trên hoặc chạy `dotnet dev-certs https --trust`.
