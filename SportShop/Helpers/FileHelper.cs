namespace SportShop.Helpers
{
    public static class FileHelper
    {
        // ==========================================
        // HÀM LÕI (CORE): KIỂM TRA BẢO MẬT & LƯU FILE
        // ==========================================
        private static async Task<string> UploadFileCoreAsync(IFormFile file, string webRootPath, string baseFolder, string subFolder, string prefixName, string[] allowedExtensions, string[] allowedMimeTypes, int maxFileSizeMb = 5)
        {
            if (file == null || file.Length == 0) return null;

            // 1. Kiểm tra dung lượng
            long maxFileSize = maxFileSizeMb * 1024 * 1024;
            if (file.Length > maxFileSize)
                throw new Exception($"Kích thước file không được vượt quá {maxFileSizeMb}MB.");

            // 2. Kiểm tra đuôi file (Chống upload shell/script)
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
                throw new Exception($"Chỉ cho phép tải lên định dạng: {string.Join(", ", allowedExtensions)}");

            // 3. Kiểm tra MIME Type (Chống đổi đuôi file, vd: virus.exe -> virus.png)
            if (!allowedMimeTypes.Contains(file.ContentType.ToLower()))
                throw new Exception("Định dạng nội dung file không hợp lệ hoặc file bị giả mạo.");

            // 4. Tạo tên file theo chuẩn: [Tiền_tố]_[Timestamp][Đuôi_file]
            string fileName = $"{prefixName}_{DateTime.Now:yyyyMMddHHmmss}{extension}";

            // 5. Đường dẫn lưu trữ (Ví dụ: wwwroot/images/avatars)
            string uploadsFolder = Path.Combine(webRootPath, baseFolder, subFolder);

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string filePath = Path.Combine(uploadsFolder, fileName);

            // 6. Lưu file vật lý
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // 7. Trả về đường dẫn để lưu vào Database
            return $"/{baseFolder}/{subFolder}/{fileName}";
        }

        // ==========================================
        // MODULE 1: XỬ LÝ HÌNH ẢNH (AVATAR, PRODUCT...)
        // ==========================================
        public static async Task<string> UploadImageAsync(IFormFile file, string webRootPath, string folderName, string prefixName)
        {
            var allowedExts = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var allowedMimes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };

            // Giới hạn ảnh 5MB, lưu vào /images/
            return await UploadFileCoreAsync(file, webRootPath, "images", folderName, prefixName, allowedExts, allowedMimes, 5);
        }

        public static async Task<List<string>> UploadMultipleImagesAsync(List<IFormFile> files, string webRootPath, string folderName, string prefixName)
        {
            var uploadedPaths = new List<string>();
            if (files == null || !files.Any()) return uploadedPaths;

            foreach (var file in files)
            {
                string uniquePrefix = $"{prefixName}_{Guid.NewGuid().ToString("N").Substring(0, 6)}";
                var path = await UploadImageAsync(file, webRootPath, folderName, uniquePrefix);
                if (!string.IsNullOrEmpty(path)) uploadedPaths.Add(path);
            }
            return uploadedPaths;
        }

        // ==========================================
        // MODULE 2: XỬ LÝ TÀI LIỆU EXCEL (DÀNH CHO ADMIN)
        // ==========================================
        public static async Task<string> UploadExcelAsync(IFormFile file, string webRootPath, string folderName, string prefixName)
        {
            var allowedExts = new[] { ".xlsx", ".xls", ".csv" };
            var allowedMimes = new[] {
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", // .xlsx
                "application/vnd.ms-excel", // .xls
                "text/csv" // .csv
            };

            // Giới hạn file Excel tối đa 10MB, lưu vào /documents/
            return await UploadFileCoreAsync(file, webRootPath, "documents", folderName, prefixName, allowedExts, allowedMimes, 10);
        }

        // ==========================================
        // MODULE 3: DỌN DẸP RÁC (XÓA FILE)
        // ==========================================
        public static void DeleteFile(string webRootPath, string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath)) return;

            var cleanPath = relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            var filePath = Path.Combine(webRootPath, cleanPath);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}