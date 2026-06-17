using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SportShop.Data.Repositories;
using SportShop.Entities;
using SportShop.Entities.Shop;
using SportShop.ViewModels;
using SportShop.ViewModels.Home;
using SportShop.ViewModels.Shared;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SportShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        public HomeController(IUnitOfWork unitOfWork, IMapper mapper, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                // Lấy thông tin người dùng từ Database
                var user = await _userManager.GetUserAsync(User);

                // Nếu là Admin -> Tự động bẻ lái sang Dashboard Admin
                if (user != null && user.Role == UserRole.Admin)
                {
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
            }

            var viewModel = new HomeViewModel();

            var featuredProducts = await _unitOfWork.Product.GetFeaturedProductsAsync(8, includeProperties: "Sport,Category,Brand,ProductImages");
            viewModel.FeaturedProducts = _mapper.Map<List<ProductCardViewModel>>(featuredProducts);

            var categories = await _unitOfWork.Category.GetAllAsync();

            foreach (var cat in categories.Take(4))
            {
                var catProducts = await _unitOfWork.Product.GetProductsByCategoryAsync(cat.Id, 4, includeProperties: "Sport,Category,Brand,ProductImages");

                if (catProducts.Any())
                {
                    viewModel.CategorySections.Add(new CategorySectionViewModel
                    {
                        CategoryId = cat.Id,
                        CategoryName = cat.Name,
                        Products = _mapper.Map<List<ProductCardViewModel>>(catProducts)
                    });
                }
            }

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}