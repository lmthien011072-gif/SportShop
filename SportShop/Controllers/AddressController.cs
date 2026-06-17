using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SportShop.Data.Repositories;
using SportShop.Entities;
using SportShop.Entities.Shop;
using SportShop.ViewModels.User;
using System.Linq;
using System.Threading.Tasks;

namespace SportShop.Controllers
{
    [Authorize]
    public class AddressController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public AddressController(IUnitOfWork unitOfWork, UserManager<User> userManager, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var addresses = await _unitOfWork.UserAddress.GetAllAsync(filter: a => a.UserId == user.Id);
            return View(addresses.OrderByDescending(a => a.IsDefault).ToList());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(AddressViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var existingAddresses = await _unitOfWork.UserAddress.GetAllAsync(filter: a => a.UserId == user.Id);
                bool hasAddress = existingAddresses.Any();

                var newAddress = _mapper.Map<UserAddress>(model);

                newAddress.UserId = user.Id;
                newAddress.IsDefault = !hasAddress || model.IsDefault;

                if (newAddress.IsDefault && hasAddress)
                {
                    var oldDefaults = existingAddresses.Where(a => a.IsDefault).ToList();
                    foreach (var addr in oldDefaults)
                    {
                        addr.IsDefault = false;
                        _unitOfWork.UserAddress.Update(addr);
                    }
                }

                await _unitOfWork.UserAddress.AddAsync(newAddress);
                await _unitOfWork.SaveAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetDefault(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var addresses = await _unitOfWork.UserAddress.GetAllAsync(filter: a => a.UserId == user.Id);

            if (addresses.Any())
            {
                var targetAddress = addresses.FirstOrDefault(a => a.Id == id);
                if (targetAddress != null && !targetAddress.IsDefault)
                {
                    foreach (var addr in addresses)
                    {
                        addr.IsDefault = false;
                        _unitOfWork.UserAddress.Update(addr);
                    }
                    targetAddress.IsDefault = true;
                    _unitOfWork.UserAddress.Update(targetAddress);
                    await _unitOfWork.SaveAsync();
                }
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var address = await _unitOfWork.UserAddress.GetFirstOrDefaultAsync(filter: a => a.Id == id && a.UserId == user.Id);
            if (address == null) return NotFound();
            return View(address);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserAddress model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var existingAddress = await _unitOfWork.UserAddress.GetFirstOrDefaultAsync(filter: a => a.Id == model.Id && a.UserId == user.Id);

                if (existingAddress != null)
                {
                    _mapper.Map(model, existingAddress);

                    _unitOfWork.UserAddress.Update(existingAddress);
                    await _unitOfWork.SaveAsync();

                    return RedirectToAction(nameof(Index));
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var address = await _unitOfWork.UserAddress.GetFirstOrDefaultAsync(filter: a => a.Id == id && a.UserId == user.Id);

            if (address != null && !address.IsDefault)
            {
                _unitOfWork.UserAddress.Remove(address);
                await _unitOfWork.SaveAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}