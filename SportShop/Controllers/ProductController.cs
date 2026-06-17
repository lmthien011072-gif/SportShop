using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SportShop.Data.Repositories;
using SportShop.ViewModels.Product;
using SportShop.ViewModels.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportShop.Controllers
{
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IActionResult> Details(int id)
        {
            // 1. Lấy thông tin sản phẩm chính
            var product = await _unitOfWork.Product.GetFirstOrDefaultAsync(
                filter: p => p.Id == id,
                includeProperties: "Sport,Category,Brand,ProductImages,ProductVariants"
            );

            if (product == null)
            {
                return NotFound();
            }

            var viewModel = _mapper.Map<ProductDetailViewModel>(product);

            // 2. Lấy Sản phẩm liên quan (Cùng Danh mục, loại trừ sản phẩm hiện tại)
            var relatedProductsDb = await _unitOfWork.Product.GetAllAsync(
                filter: p => p.CategoryId == product.CategoryId && p.Id != id,
                includeProperties: "Sport,Category,Brand,ProductImages"
            );

            // Lấy 4 sản phẩm ngẫu nhiên hoặc mới nhất
            var relatedProductCards = _mapper.Map<List<ProductCardViewModel>>(
                relatedProductsDb.OrderByDescending(p => p.Id).Take(4).ToList()
            );

            viewModel.RelatedProducts = relatedProductCards;

            return View(viewModel);
        }
    }
}