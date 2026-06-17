using AutoMapper;
using SportShop.Entities.Shop;
using SportShop.ViewModels.Shared;
using SportShop.ViewModels.Product;
using SportShop.ViewModels.Order;
using SportShop.ViewModels.User;
using SportShop.Areas.Admin.ViewModels; // BỔ SUNG: Khai báo namespace cho các ViewModel của Admin
using System.Linq;
using SportShop.Areas.Admin.ViewModels.Catalog;
using SportShop.Areas.Admin.ViewModels.Order;
using SportShop.Areas.Admin.ViewModels.Product;

namespace SportShop.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // ========================================================
            // 1. ÁNH XẠ SẢN PHẨM & BIẾN THỂ (CUSTOMER AREA)
            // ========================================================
            CreateMap<Product, ProductDetailViewModel>()
                // Lấy tên từ các thực thể liên quan
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand.Name))
                .ForMember(dest => dest.SportName, opt => opt.MapFrom(src => src.Sport.Name))

                // Tự động bóc tách danh sách URL từ danh sách đối tượng ProductImage
                .ForMember(dest => dest.GalleryImages, opt => opt.MapFrom(src =>
                    src.ProductImages.Select(img => img.ImageUrl).ToList()))

                // Logic lấy ảnh đại diện
                .ForMember(dest => dest.MainImageUrl, opt => opt.MapFrom(src =>
                    !string.IsNullOrEmpty(src.ThumbnailUrl) ? src.ThumbnailUrl :
                    (src.ProductImages.FirstOrDefault() != null ? src.ProductImages.First().ImageUrl : "/images/no-image.png")))

                // Map danh sách biến thể sản phẩm
                .ForMember(dest => dest.AvailableVariants, opt => opt.MapFrom(src => src.ProductVariants));

            // Mapping cho biến thể sản phẩm
            CreateMap<ProductVariant, VariantOptionViewModel>()
                .ForMember(dest => dest.VariantId, opt => opt.MapFrom(src => src.Id))
                // Logic tìm ảnh thuộc về biến thể, fallback về ảnh gốc của sản phẩm
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src =>
                    src.Product.ProductImages.FirstOrDefault(pi => pi.ProductVariantId == src.Id) != null
                    ? src.Product.ProductImages.FirstOrDefault(pi => pi.ProductVariantId == src.Id).ImageUrl
                    : src.Product.ThumbnailUrl));

            // Mapping cho thẻ sản phẩm (dùng trong danh sách sản phẩm liên quan và Trang chủ)
            CreateMap<Product, ProductCardViewModel>()
                .ForMember(dest => dest.ThumbnailUrl, opt => opt.MapFrom(src =>
                    !string.IsNullOrEmpty(src.ThumbnailUrl) ? src.ThumbnailUrl :
                    (src.ProductImages.FirstOrDefault() != null ? src.ProductImages.First().ImageUrl : "/images/no-image.png")));


            // ========================================================
            // 2. ÁNH XẠ ĐƠN HÀNG VÀ CHI TIẾT ĐƠN HÀNG (CUSTOMER AREA)
            // ========================================================

            // Ánh xạ Order -> OrderHistoryViewModel
            CreateMap<Order, OrderHistoryViewModel>()
                .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.Status));

            // Ánh xạ Order -> OrderDetailViewModel
            CreateMap<Order, OrderDetailViewModel>()
                .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.OrderItems));

            // Ánh xạ OrderItem -> OrderItemViewModel
            CreateMap<OrderItem, OrderItemViewModel>()
                // Xử lý an toàn fallback nếu ProductVariant hoặc Product bị null
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductVariant.Product.Name ?? "Sản phẩm không xác định"))
                .ForMember(dest => dest.ThumbnailUrl, opt => opt.MapFrom(src => src.ProductVariant.Product.ThumbnailUrl ?? "https://placehold.co/100x100"))
                .ForMember(dest => dest.Size, opt => opt.MapFrom(src => src.ProductVariant.Size))
                .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.ProductVariant.Color))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.UnitPrice > 0 ? src.UnitPrice : (src.ProductVariant.Product != null ? src.ProductVariant.Product.Price : 0)));


            // ========================================================
            // 3. ÁNH XẠ TÀI KHOẢN VÀ ĐỊA CHỈ NGƯỜI DÙNG (CUSTOMER AREA)
            // ========================================================

            // Map từ Entity User sang ViewModel để hiển thị Profile
            CreateMap<User, UserProfileViewModel>().ReverseMap();

            // Map từ ViewModel (Form tạo mới) sang Entity
            CreateMap<AddressViewModel, UserAddress>();

            // Map đè từ Entity (Form Edit gửi lên) sang Entity (Trong Database)
            CreateMap<UserAddress, UserAddress>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.IsDefault, opt => opt.Ignore());


            // ========================================================
            // 4. ÁNH XẠ KHU VỰC ADMIN (ADMIN AREA) - DỮ LIỆU MỚI
            // ========================================================

            // 4.1. Catalog (Thương hiệu, Danh mục, Môn thể thao)
            CreateMap<Brand, BrandAdminViewModel>().ReverseMap();
            CreateMap<Category, CategoryAdminViewModel>().ReverseMap();
            CreateMap<Sport, SportAdminViewModel>().ReverseMap();

            // 4.2. Sản phẩm (Product)
            CreateMap<Product, ProductAdminViewModel>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : ""))
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand != null ? src.Brand.Name : ""))
                .ForMember(dest => dest.SportName, opt => opt.MapFrom(src => src.Sport != null ? src.Sport.Name : ""));
            CreateMap<ProductAdminViewModel, Product>();

            CreateMap<ProductVariant, ProductVariantAdminViewModel>().ReverseMap();
            CreateMap<ProductImage, ProductImageAdminViewModel>().ReverseMap();

            // 4.3. Đơn hàng (Order)
            // Ánh xạ cho danh sách rút gọn (Order List)
            CreateMap<Order, OrderListViewModel>()
                // Lưu ý: Vì Order.cs của bạn chỉ có UserId (Không có navigation property User), 
                // ta tạm map Tên Khách hàng thông qua UserId. Khi truy vấn trong Controller, ta có thể join với User sau.
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => "Khách hàng #" + src.UserId));

            // Ánh xạ cho chi tiết đơn hàng quản trị (có OrderItems)
            CreateMap<Order, OrderDetailAdminViewModel>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.OrderItems))
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => "Khách hàng #" + src.UserId));

            // Ánh xạ chi tiết sản phẩm trong đơn hàng
            CreateMap<OrderItem, OrderItemAdminViewModel>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductVariant.Product != null ? src.ProductVariant.Product.Name : "Sản phẩm đã xóa"))
                .ForMember(dest => dest.Size, opt => opt.MapFrom(src => src.ProductVariant.Size))
                .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.ProductVariant.Color))
                .ForMember(dest => dest.ThumbnailUrl, opt => opt.MapFrom(src => src.ProductVariant.Product != null ? src.ProductVariant.Product.ThumbnailUrl : ""));
        }
    }
}