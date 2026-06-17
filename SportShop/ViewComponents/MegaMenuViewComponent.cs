using Microsoft.AspNetCore.Mvc;
using SportShop.Services;
using System.Threading.Tasks;

namespace SportShop.ViewComponents
{
    public class MegaMenuViewComponent : ViewComponent
    {
        private readonly ICatalogService _catalogService;

        public MegaMenuViewComponent(ICatalogService catalogService)
        {
            _catalogService = catalogService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var menuItems = await _catalogService.GetMegaMenuAsync();
            return View(menuItems);
        }
    }
}