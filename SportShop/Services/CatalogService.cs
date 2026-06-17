using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using SportShop.Data.Repositories;
using SportShop.ViewModels.Catalog;
using SportShop.ViewModels.Menu;
using SportShop.ViewModels.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SportShop.Services
{
    public interface ICatalogService
    {
        Task<CatalogViewModel> GetCatalogAsync(int? categoryId, int? sportId, int? brandId);

        Task<List<MegaMenuItemViewModel>> GetMegaMenuAsync();
    }

    public class CatalogService : ICatalogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache;

        public CatalogService(IUnitOfWork unitOfWork, IMapper mapper, IMemoryCache memoryCache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _memoryCache = memoryCache;
        }

        public async Task<CatalogViewModel> GetCatalogAsync(int? categoryId, int? sportId, int? brandId)
        {
            var viewModel = new CatalogViewModel
            {
                ActiveCategoryId = categoryId,
                ActiveSportId = sportId,
                ActiveBrandId = brandId
            };

            // 1. LẤY THÔNG TIN TÊN CỦA CÁC BỘ LỌC ĐANG ACTIVE
            if (categoryId.HasValue)
            {
                var category = await _unitOfWork.Category.GetByIdAsync(categoryId.Value);
                viewModel.ActiveCategoryName = category?.Name;
            }
            if (sportId.HasValue)
            {
                var sport = await _unitOfWork.Sport.GetByIdAsync(sportId.Value);
                viewModel.ActiveSportName = sport?.Name;
            }
            if (brandId.HasValue)
            {
                var brand = await _unitOfWork.Brand.GetByIdAsync(brandId.Value);
                viewModel.ActiveBrandName = brand?.Name;
                viewModel.ActiveBrandLogoUrl = brand?.LogoUrl;
            }

            // 2. QUERY SẢN PHẨM THỰC TẾ (Lọc theo TẤT CẢ tiêu chí)
            var products = await _unitOfWork.Product.GetAllAsync(
                filter: p => (!categoryId.HasValue || p.CategoryId == categoryId.Value) &&
                             (!sportId.HasValue || p.SportId == sportId.Value) &&
                             (!brandId.HasValue || p.BrandId == brandId.Value),
                includeProperties: "Category,Brand,Sport,ProductImages"
            );
            viewModel.Products = _mapper.Map<List<ProductCardViewModel>>(products);

            // 3. XÂY DỰNG BỘ LỌC ĐỘNG (MA THUẬT NẰM Ở ĐÂY)
            // Lấy danh mục dựa trên Hãng/Môn đang chọn (Bỏ qua CategoryId)
            var catQuery = await _unitOfWork.Product.GetAllAsync(
                filter: p => (!sportId.HasValue || p.SportId == sportId.Value) &&
                             (!brandId.HasValue || p.BrandId == brandId.Value),
                includeProperties: "Category"
            );
            viewModel.AvailableCategories = catQuery.Where(p => p.Category != null)
                .Select(p => p.Category).GroupBy(c => c.Id).Select(g => g.First())
                .Select(c => new CategoryMenuItem { CategoryId = c.Id, CategoryName = c.Name }).ToList();

            // Lấy Môn thể thao dựa trên Hãng/Danh mục (Bỏ qua SportId)
            var sportQuery = await _unitOfWork.Product.GetAllAsync(
                filter: p => (!categoryId.HasValue || p.CategoryId == categoryId.Value) &&
                             (!brandId.HasValue || p.BrandId == brandId.Value),
                includeProperties: "Sport"
            );
            viewModel.AvailableSports = sportQuery.Where(p => p.Sport != null)
                .Select(p => p.Sport).GroupBy(s => s.Id).Select(g => g.First())
                .Select(s => new SportMenuItem { SportId = s.Id, SportName = s.Name }).ToList();

            // Lấy Hãng dựa trên Môn/Danh mục (Bỏ qua BrandId)
            var brandQuery = await _unitOfWork.Product.GetAllAsync(
                filter: p => (!categoryId.HasValue || p.CategoryId == categoryId.Value) &&
                             (!sportId.HasValue || p.SportId == sportId.Value),
                includeProperties: "Brand"
            );
            viewModel.AvailableBrands = brandQuery.Where(p => p.Brand != null)
                .Select(p => p.Brand).GroupBy(b => b.Id).Select(g => g.First())
                .Select(b => new BrandMenuItem { BrandId = b.Id, BrandName = b.Name, LogoUrl = b.LogoUrl }).ToList();

            return viewModel;
        }

        public async Task<List<MegaMenuItemViewModel>> GetMegaMenuAsync()
        {
            const string cacheKey = "MegaMenuCacheKey";

            if (!_memoryCache.TryGetValue(cacheKey, out List<MegaMenuItemViewModel> menuItems))
            {
                var categories = await _unitOfWork.Category.GetAllAsync();
                var products = await _unitOfWork.Product.GetAllAsync(includeProperties: "Brand,Sport");

                menuItems = new List<MegaMenuItemViewModel>();

                foreach (var cat in categories)
                {
                    var brandsInCategory = products.Where(p => p.CategoryId == cat.Id && p.Brand != null)
                        .Select(p => p.Brand).GroupBy(b => b.Id).Select(g => g.First())
                        .Select(b => new BrandMenuItem { BrandId = b.Id, BrandName = b.Name, LogoUrl = b.LogoUrl }).ToList();

                    var sportsInCategory = products.Where(p => p.CategoryId == cat.Id && p.Sport != null)
                        .Select(p => p.Sport).GroupBy(s => s.Id).Select(g => g.First())
                        .Select(s => new SportMenuItem { SportId = s.Id, SportName = s.Name }).ToList();

                    // Gán icon tự động
                    string icon = "⚽";
                    if (cat.Name.ToLower().Contains("vợt")) icon = "🏸";
                    else if (cat.Name.ToLower().Contains("giày")) icon = "👟";
                    else if (cat.Name.ToLower().Contains("áo") || cat.Name.ToLower().Contains("quần")) icon = "👕";
                    else if (cat.Name.ToLower().Contains("balo")) icon = "🎒";

                    menuItems.Add(new MegaMenuItemViewModel
                    {
                        CategoryId = cat.Id,
                        CategoryName = cat.Name,
                        IconClass = icon,
                        RelatedBrands = brandsInCategory,
                        RelatedSports = sportsInCategory
                    });
                }
                _memoryCache.Set(cacheKey, menuItems, System.TimeSpan.FromMinutes(60));
            }
            return menuItems;
        }
    }
}