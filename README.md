# Hướng dẫn Cài đặt và Chạy Dự án SportShop

# 

# Chào mừng bạn đến với dự án SportShop. Để đảm bảo chương trình chạy ngay mà không gặp lỗi, vui lòng làm theo đúng các bước dưới đây.

# 

# 1\. Yêu cầu hệ thống

# 

# Công cụ: Visual Studio 2022 (hoặc bản mới hơn) và SQL Server Management Studio (SSMS).

# 

# Database: SQL Server.

# 

# 2\. Thiết lập Database (Cần thực hiện trước)

# 

# Dự án cần 2 cơ sở dữ liệu để hoạt động. Bạn hãy thực hiện theo cách thuận tiện nhất:

# 

# Cách 1: Sử dụng Script SQL (Khuyên dùng - Dễ nhất)

# 

# Mở SQL Server Management Studio (SSMS) và kết nối vào Server của bạn.

# 

# Tạo 2 Database trống với tên chính xác: SportShopDB và VietNamAddressDB.

# 

# Chuột phải vào SportShopDB -> New Query -> Mở file SportShopDB.sql (từ thư mục DatabaseScripts/) -> Nhấn Execute.

# 

# Lặp lại tương tự với VietNamAddressDB bằng cách chạy file VietNamAddressDB.sql.

# 

# Cách 2: Attach file .mdf (Nếu có sẵn file dữ liệu)

# 

# Copy 2 file .mdf và .ldf từ thư mục DatabaseScripts/ vào một thư mục cố định trên máy bạn (ví dụ: C:\\Databases\\).

# 

# Trong SSMS, chuột phải vào mục Databases -> Chọn Attach...

# 

# Nhấn Add và chọn lần lượt các file .mdf vừa copy. Nhấn OK.

# 

# 3\. Cấu hình Chuỗi Kết nối (Connection String)

# 

# Sau khi có database, bạn cần trỏ project tới database của mình:

# 

# Mở file appsettings.json trong thư mục gốc của project.

# 

# Tìm mục ConnectionStrings và sửa YOUR\_SERVER\_NAME thành tên Server SQL của bạn (thường là . hoặc localhost hoặc tên máy tính):

# 

# {

# &#x20; "ConnectionStrings": {

# &#x20;   "SportShopConnection": "Server=.;Database=SportShopDB;Trusted\_Connection=True;MultipleActiveResultSets=true",

# &#x20;   "AddressConnection": "Server=.;Database=VietNamAddressDB;Trusted\_Connection=True;MultipleActiveResultSets=true"

# &#x20; }

# }

# 

# 

# 4\. Chạy chương trình

# 

# Mở file giải pháp .sln bằng Visual Studio.

# 

# Build Solution: Nhấn Ctrl + Shift + B để đảm bảo không có lỗi biên dịch.

# 

# Chạy ứng dụng: Nhấn F5 hoặc nút Run (biểu tượng tam giác xanh) trên thanh công cụ.

# 

# Trình duyệt mặc định sẽ mở lên và đưa bạn đến trang chủ.

# 

# 5\. Khắc phục sự cố thường gặp

# 

# Lỗi không kết nối được Database: Kiểm tra lại tên Server trong appsettings.json. Đảm bảo dịch vụ SQL Server đang chạy (Start SQL Server Services).

# 

# Lỗi thiếu dữ liệu: Nếu bạn dùng cách Attach file nhưng không thấy dữ liệu, hãy thử kiểm tra xem user sa hoặc tài khoản Windows hiện tại đã có quyền db\_owner trên các database đó chưa.

# 

# Project không build được: Hãy nhấn chuột phải vào Solution trong Solution Explorer -> Restore NuGet Packages.

# 

# Chúc bạn cài đặt thành công! Nếu cần hỗ trợ thêm, vui lòng kiểm tra lại kết nối SQL Server của bạn.

