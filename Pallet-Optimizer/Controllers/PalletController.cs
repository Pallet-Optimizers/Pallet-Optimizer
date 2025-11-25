using Microsoft.AspNetCore.Mvc;
using Pallet_Optimizer.Data;
using Pallet_Optimizer.Models;
using System.Threading.Tasks;
using Pallet_Optimizer.Filters;

namespace Pallet_Optimizer.Controllers
{
    [AuthorizeSession]
    public class PalletController : Controller
    {
        private readonly IPalletRepository _repo;

        public PalletController(IPalletRepository repo)
        {
            _repo = repo;
        }

        // GET: /Pallet?planId=xxx
        public async Task<IActionResult> Index(string? planId)
        {
            PalletHolder holder;

            if (!string.IsNullOrEmpty(planId))
            {
                holder = await _repo.GetPackagePlanByIdAsync(planId);
                if (holder == null)
                {
                    TempData["Error"] = "Pakkeplan ikke fundet";
                    return RedirectToAction("Index", "Dashboard");
                }
            }
            else
            {
                holder = await _repo.GetHolderAsync();
            }

            return View(holder);
        }

        // POST: /Pallet/UpdatePallet
        [HttpPost]
        public async Task<IActionResult> UpdatePallet(UpdatePalletDto dto)
        {
            var pallet = await _repo.GetPalletAsync(dto.Index);
            if (pallet == null) return NotFound();

            pallet.MaterialType = (PALLET_MATERIAL_TYPE)dto.MaterialType;
            await _repo.UpdatePalletAsync(dto.Index, pallet);

            return Json(new { success = true, index = dto.Index, material = pallet.MaterialType.ToString() });
        }

        // GET: /Pallet/GetPallet
        [HttpGet]
        public async Task<IActionResult> GetPallet(int index)
        {
            var pallet = await _repo.GetPalletAsync(index);
            if (pallet == null) return NotFound();
            return Json(pallet);
        }
    }

    public class UpdatePalletDto
    {
        public int Index { get; set; }
        public PALLET_MATERIAL_TYPE MaterialType { get; set; }
    }
}