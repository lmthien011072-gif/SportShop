using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportShop.Data;

namespace SportShop.Controllers.Api
{
    // Cấu hình Route: /api/address
    [Route("api/address")]
    [ApiController] // Đánh dấu đây là API, nó sẽ tự động serialize kết quả thành JSON
    public class AddressApiController : ControllerBase
    {
        // Đổi từ ApplicationDbContext sang AddressDbContext
        private readonly AddressDbContext _context;

        public AddressApiController(AddressDbContext context)
        {
            _context = context;
        }

        // Endpoint 1: Lấy danh sách Tỉnh/Thành
        // GET: /api/address/provinces
        [HttpGet("provinces")]
        public async Task<IActionResult> GetProvinces()
        {
            var provinces = await _context.Provinces
                .Select(p => new { code = p.Code, name = p.Name })
                .OrderBy(p => p.name)
                .ToListAsync();

            return Ok(provinces); // Trả về HTTP 200 kèm data JSON
        }

        // Endpoint 2: Lấy danh sách Xã/Phường theo mã Tỉnh
        // GET: /api/address/provinces/79/communes
        [HttpGet("provinces/{provinceCode}/communes")]
        public async Task<IActionResult> GetCommunes(string provinceCode)
        {
            var communes = await _context.Communes
                .Where(c => c.ProvinceCode == provinceCode)
                .Select(c => new { code = c.Code, name = c.Name })
                .OrderBy(c => c.name)
                .ToListAsync();

            return Ok(communes);
        }
    }
}