using Microsoft.AspNetCore.Mvc;
using SportShop.Services;
using System.Threading.Tasks;

namespace SportShop.Controllers
{
    public class CatalogController : Controller
    {
        private readonly ICatalogService _catalogService;

        public CatalogController(ICatalogService catalogService)
        {
            _catalogService = catalogService;
        }

        public async Task<IActionResult> Index(int? categoryId, int? sportId, int? brandId)
        {
            var viewModel = await _catalogService.GetCatalogAsync(categoryId, sportId, brandId);

            if (viewModel == null) return NotFound();

            return View(viewModel);
        }
    }
}