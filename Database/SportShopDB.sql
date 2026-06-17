-- =========================================================
-- FILE NẠP DỮ LIỆU MẪU CHO SPORT SHOP (PHIÊN BẢN CÓ BẢNG SPORTS)
-- Cách dùng: Copy toàn bộ nội dung này, mở SQL Server Management Studio (SSMS),
-- Bấm nút "New Query", paste vào và bấm "Execute" (F5)
-- =========================================================

USE [SportShopDB]; -- Đổi tên này nếu Database của bạn tên khác
GO

-- 1. XÓA DỮ LIỆU CŨ (Nếu có) ĐỂ TRÁNH LỖI TRÙNG LẶP
-- Lưu ý: Xóa từ bảng con đến bảng cha
DELETE FROM [CartItems];
DELETE FROM [OrderItems];
DELETE FROM [ProductVariants];
DELETE FROM [ProductImages];
DELETE FROM [Products];
DELETE FROM [Categories];
DELETE FROM [Brands];
DELETE FROM [Sports]; -- Xóa thêm bảng Sports

-- Reset lại cột ID tự động tăng về 1
DBCC CHECKIDENT ('[Sports]', RESEED, 0);
DBCC CHECKIDENT ('[Categories]', RESEED, 0);
DBCC CHECKIDENT ('[Brands]', RESEED, 0);
DBCC CHECKIDENT ('[Products]', RESEED, 0);
DBCC CHECKIDENT ('[ProductVariants]', RESEED, 0);
GO

-- =========================================================
-- 2. THÊM DỮ LIỆU MÔN THỂ THAO (SPORTS)
-- =========================================================
INSERT INTO [Sports] ([Name]) VALUES 
(N'Cầu Lông'),      -- ID: 1
(N'Bóng Đá'),       -- ID: 2
(N'Chạy Bộ'),       -- ID: 3
(N'Bóng Rổ');       -- ID: 4

-- =========================================================
-- 3. THÊM DỮ LIỆU THƯƠNG HIỆU (BRANDS)
-- =========================================================
INSERT INTO [Brands] ([Name], [LogoUrl]) VALUES 
(N'Yonex', 'https://placehold.co/150x50/1a1a2e/ffffff?text=YONEX'),  -- 1
(N'Victor', 'https://placehold.co/150x50/1a1a2e/ffffff?text=VICTOR'), -- 2
(N'Lining', 'https://placehold.co/150x50/1a1a2e/ffffff?text=LINING'), -- 3
(N'Nike', 'https://placehold.co/150x50/1a1a2e/ffffff?text=NIKE'),    -- 4
(N'Adidas', 'https://placehold.co/150x50/1a1a2e/ffffff?text=ADIDAS'); -- 5

-- =========================================================
-- 4. THÊM DỮ LIỆU DANH MỤC (CATEGORIES)
-- =========================================================
INSERT INTO [Categories] ([Name]) VALUES 
(N'Vợt'),             -- ID: 1
(N'Giày'),            -- ID: 2
(N'Quần Áo'),         -- ID: 3
(N'Balo - Túi Xách'), -- ID: 4
(N'Phụ Kiện');        -- ID: 5

-- =========================================================
-- 5. THÊM DỮ LIỆU SẢN PHẨM (PRODUCTS)
-- =========================================================
-- (SportId, CategoryId, BrandId, Name, Price, ThumbnailUrl, Description)
INSERT INTO [Products] ([SportId], [CategoryId], [BrandId], [Name], [Price], [ThumbnailUrl], [Description]) VALUES 
-- SẢN PHẨM CẦU LÔNG (SportId: 1)
(1, 1, 1, N'Vợt Cầu Lông Yonex Astrox 99 Pro', 3500000, 'https://placehold.co/400x400/f36f21/ffffff?text=Astrox+99+Pro', N'Vợt tấn công mạnh mẽ dành cho vận động viên chuyên nghiệp.'),
(1, 1, 2, N'Vợt Cầu Lông Victor Thruster Ryuga', 3200000, 'https://placehold.co/400x400/e94560/ffffff?text=Victor+Ryuga', N'Vợt thiên công của Lee Zii Jia.'),
(1, 2, 1, N'Giày Cầu Lông Yonex 65Z3 Men', 2500000, 'https://placehold.co/400x400/f36f21/ffffff?text=Yonex+65Z3', N'Giày cầu lông quốc dân, bám sân cực tốt.'),
(1, 2, 3, N'Giày Cầu Lông Lining AYAS018', 1800000, 'https://placehold.co/400x400/e94560/ffffff?text=Lining+AYAS', N'Giày siêu nhẹ, êm ái.'),
(1, 4, 1, N'Balo Cầu Lông Yonex BA92012', 1200000, 'https://placehold.co/400x400/1a1a2e/ffffff?text=Balo+Yonex', N'Balo chuyên dụng đựng vợt và giày.'),

-- SẢN PHẨM BÓNG ĐÁ (SportId: 2)
(2, 2, 4, N'Giày Bóng Đá Nike Mercurial Vapor 15', 4500000, 'https://placehold.co/400x400/1a1a2e/ffffff?text=Nike+Mercurial', N'Giày đá bóng sân cỏ nhân tạo, bứt tốc nhanh.'),
(2, 2, 5, N'Giày Bóng Đá Adidas X Crazyfast', 3800000, 'https://placehold.co/400x400/f36f21/ffffff?text=Adidas+X', N'Trọng lượng siêu nhẹ, cảm giác bóng tốt.'),
(2, 5, 4, N'Găng Tay Thủ Môn Nike Goalkeeper', 950000, 'https://placehold.co/400x400/e94560/ffffff?text=Nike+Gloves', N'Độ bám dính cực cao.'),

-- SẢN PHẨM CHẠY BỘ (SportId: 3)
(3, 2, 4, N'Giày Chạy Bộ Nike Pegasus 40', 3200000, 'https://placehold.co/400x400/f36f21/ffffff?text=Nike+Pegasus', N'Giày chạy bộ êm ái, phù hợp chạy hàng ngày.'),
(3, 3, 5, N'Áo Chạy Bộ Adidas Own The Run', 850000, 'https://placehold.co/400x400/1a1a2e/ffffff?text=Adidas+Run+Shirt', N'Công nghệ AEROREADY thấm hút mồ hôi.');

-- =========================================================
-- 6. THÊM DỮ LIỆU BIẾN THỂ - SIZE/MÀU (PRODUCT VARIANTS)
-- =========================================================
-- Vợt Yonex Astrox (ProductId: 1)
INSERT INTO [ProductVariants] ([ProductId], [Size], [Color], [StockQuantity]) VALUES 
(1, '4U/G5', N'Đỏ/Trắng', 10),
(1, '3U/G5', N'Đỏ/Trắng', 5);

-- Giày Yonex 65Z3 (ProductId: 3)
INSERT INTO [ProductVariants] ([ProductId], [Size], [Color], [StockQuantity]) VALUES 
(3, '40', N'Trắng', 15),
(3, '41', N'Trắng', 20),
(3, '42', N'Trắng', 8);

-- Giày Nike Mercurial (ProductId: 6)
INSERT INTO [ProductVariants] ([ProductId], [Size], [Color], [StockQuantity]) VALUES 
(6, '40', N'Xanh Chuối', 12),
(6, '41', N'Xanh Chuối', 18);

-- Áo Adidas Own The Run (ProductId: 10)
INSERT INTO [ProductVariants] ([ProductId], [Size], [Color], [StockQuantity]) VALUES 
(10, 'M', N'Đen', 30),
(10, 'L', N'Đen', 25),
(10, 'XL', N'Đen', 15);

PRINT N'ĐÃ NẠP DỮ LIỆU MẪU THÀNH CÔNG!';