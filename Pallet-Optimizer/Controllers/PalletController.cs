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
        [HttpPost]
        public async Task<IActionResult> UpdatePallet(UpdatePalletDto dto)
        {
            var pallet = await _repo.GetPalletAsync(dto.Index);
            if (pallet == null) return NotFound();

            pallet.MaterialType = dto.MaterialType;
            await _repo.UpdatePalletAsync(dto.Index, pallet);

            return Json(new { success = true, index = dto.Index, material = pallet.MaterialType.ToString() });
        }

        // GET pallet data endpoint
        [HttpGet]
        public async Task<IActionResult> GetPallet(string index)
        {
            var pallet = await _repo.GetPalletAsync(index);
            if (pallet == null) return NotFound();
            return Json(pallet);
        }

        // POST: /Pallet/AddPallet
        [HttpPost]
        public async Task<IActionResult> AddPallet(AddPalletDto dto)
        {
            var pallet = new Pallet
            {
                Id = System.Guid.NewGuid().ToString(),
                Name = dto.Name,
                MaterialType = dto.MaterialType,
                Width = 1.2,
                Length = 0.8,
                Height = 0.15,
                MaxHeight = 2.0,
                MaxWeight = 1000
            };

            await _repo.AddPalletAsync(pallet);
            return Json(new { success = true, pallet });
        }

        // POST: /Pallet/DeletePallet
        [HttpPost]
        public async Task<IActionResult> DeletePallet(DeletePalletDto dto)
        {
            await _repo.DeletePalletAsync(dto.Index);
            return Json(new { success = true, index = dto.Index });
        }

        // POST: /Pallet/AddElement
        [HttpPost]
        public async Task<IActionResult> AddElement(AddElementDto dto)
        {
            var pallet = await _repo.GetPalletAsync(dto.PalletId);
            if (pallet == null) return NotFound();

            var element = new Element
            {
                Id = System.Guid.NewGuid().ToString(),
                Name = dto.Name,
                Width = dto.Width,
                Height = dto.Height,
                Depth = dto.Depth,
                WeightKg = dto.WeightKg,
                CanRotate = dto.CanRotate,
                MustBeAlone = dto.MustBeAlone
            };

            pallet.Elements.Add(element);
            await _repo.UpdatePalletAsync(dto.PalletId, pallet);

            return Json(new { success = true, element, pallet });
        }

        // POST: /Pallet/RemoveElement
        [HttpPost]
        public async Task<IActionResult> RemoveElement(RemoveElementDto dto)
        {
            var pallet = await _repo.GetPalletAsync(dto.PalletId);
            if (pallet == null) return NotFound();

            var element = pallet.Elements.FirstOrDefault(e => e.Id == dto.ElementId);
            if (element != null)
            {
                pallet.Elements.Remove(element);
                await _repo.UpdatePalletAsync(dto.PalletId, pallet);
            }

            return Json(new { success = true, pallet });
        }
    }

    public class UpdatePalletDto
    {
        public string Index { get; set; } = "";
        public PALLET_MATERIAL_TYPE MaterialType { get; set; }
    }

    public class AddPalletDto
    {
        public string Name { get; set; } = "";
        public PALLET_MATERIAL_TYPE MaterialType { get; set; } = PALLET_MATERIAL_TYPE.Wood;
    }

    public class DeletePalletDto
    {
        public string Index { get; set; } = "";
    }

    public class AddElementDto
    {
        public string PalletId { get; set; } = "";
        public string Name { get; set; } = "";
        public double Width { get; set; }
        public double Height { get; set; }
        public double Depth { get; set; }
        public double WeightKg { get; set; }
        public bool CanRotate { get; set; } = true;
        public bool MustBeAlone { get; set; } = false;
    }

    public class RemoveElementDto
    {
        public string PalletId { get; set; } = "";
        public string ElementId { get; set; } = "";
    }
}
