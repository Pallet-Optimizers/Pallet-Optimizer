using Microsoft.AspNetCore.Mvc;
using Pallet_Optimizer.Data;
using Pallet_Optimizer.Models;
using System.Threading.Tasks;

namespace Pallet_Optimizer.Controllers
{
    public class PalletController : Controller
    {
        private readonly IPalletRepository _repo;

        public PalletController(IPalletRepository repo)
        {
            _repo = repo;
        }

        // GET: /Pallet
        public async Task<IActionResult> Index()
        {
            var holder = await _repo.GetHolderAsync();
            return View(holder);
        }

        // POST: /Pallet/UpdatePallet
        // Accept JSON body from AJAX to avoid full page reload
        [HttpPost]
        public async Task<IActionResult> UpdatePallet(UpdatePalletDto dto)
        {
            var pallet = await _repo.GetPalletAsync(dto.Index);
            if (pallet == null) return NotFound();

            // Apply changes 
            pallet.MaterialType = (PALLET_MATERIAL_TYPE)dto.MaterialType;

            // If client sends other fields, update them as well
            await _repo.UpdatePalletAsync(dto.Index, pallet);

            return Json(new { success = true, index = dto.Index, material = pallet.MaterialType.ToString() });
        }

        // GET pallet data endpoint (for client-side)
        [HttpGet]
        public async Task<IActionResult> GetPallet(int index)
        {
            var pallet = await _repo.GetPalletAsync(index);
            if (pallet == null) return NotFound();
            return Json(pallet);
        }
    }

    // TODO: Add more data here
    public class UpdatePalletDto
    {
        public int Index { get; set; }
        public PALLET_MATERIAL_TYPE MaterialType { get; set; }
    }
}
