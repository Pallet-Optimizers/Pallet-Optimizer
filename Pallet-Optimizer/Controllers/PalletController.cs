using Microsoft.AspNetCore.Mvc;
using Pallet_Optimizer.Data;
using Pallet_Optimizer.Models;
using System.Threading.Tasks;
using System.Linq;
using System.Text.Json.Serialization;
using System.Collections.Generic;

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
        public async Task<IActionResult> UpdatePallet([FromBody] UpdatePalletDto dto)
        {
            if (dto == null) return BadRequest(new { success = false, error = "Request body is required" });

            var pallet = await _repo.GetPalletAsync(dto.Index);
            if (pallet == null) return NotFound();

            pallet.MaterialType = dto.MaterialType;
            await _repo.UpdatePalletAsync(dto.Index, pallet);

            // include name & material so client can update UI
            return Json(new { success = true, index = dto.Index, material = pallet.MaterialType.ToString(), name = pallet.Name });
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
        public async Task<IActionResult> AddPallet([FromBody] AddPalletDto dto)
        {
            if (dto == null) return BadRequest(new { success = false, error = "Request body is required" });

            var pallet = new Pallet
            {
                Id = System.Guid.NewGuid().ToString(),
                Name = dto.Name,
                MaterialType = dto.MaterialType,
                Width = dto.Width,
                Length = dto.Length,
                Height = dto.Height,
                Elements = new List<Element>()
            };

            await _repo.AddPalletAsync(pallet);
            return Json(new { success = true, pallet });
        }

        // POST: /Pallet/DeletePallet
        [HttpPost]
        public async Task<IActionResult> DeletePallet([FromBody] DeletePalletDto dto)
        {
            if (dto == null) return BadRequest(new { success = false, error = "Request body is required" });

            await _repo.DeletePalletAsync(dto.Index);
            return Json(new { success = true, index = dto.Index });
        }

        // POST: /Pallet/AddElement
        [HttpPost]
        public async Task<IActionResult> AddElement([FromBody] AddElementDto dto)
        {
            if (dto == null) return BadRequest(new { success = false, error = "Request body is required" });

            var pallet = await _repo.GetPalletAsync(dto.PalletId);
            if (pallet == null) return NotFound();

            var element = new Element
            {
                Id = System.Guid.NewGuid().ToString(),
                PalletId = dto.PalletId,
                Name = dto.Name,
                Width = dto.Width,
                Height = dto.Height,
                Depth = dto.Depth,
                WeightKg = dto.WeightKg,
                CanRotate = dto.CanRotate,
                MustBeAlone = dto.MustBeAlone
            };
            Console.WriteLine($"Adding element: {element.Name}, Size: {element.Width}x{element.Height}x{element.Depth}, Weight: {dto.WeightKg}kg");
            pallet.Elements ??= new List<Element>();
            pallet.Elements.Add(element);
            await _repo.UpdatePalletAsync(dto.PalletId, pallet);

            List<Pallet> pallets = GreedyPalletOptimizer.Optimize(pallet.Elements, pallet, new OptimizationSettings());
            foreach(var pall in pallets)
            {
                await _repo.UpdatePalletAsync(pall.Id, pall);
            }
            return Json(new { success = true, element, pallets });
        }

        // POST: /Pallet/RemoveElement
        [HttpPost]
        public async Task<IActionResult> RemoveElement([FromBody] RemoveElementDto dto)
        {
            if (dto == null) return BadRequest(new { success = false, error = "Request body is required" });

            var pallet = await _repo.GetPalletAsync(dto.PalletId);
            if (pallet == null) return NotFound();

            pallet.Elements ??= new List<Element>();
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

        // accept string enum values (e.g. "Plastic") as well as numeric enum values
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PALLET_MATERIAL_TYPE MaterialType { get; set; }
    }

    public class AddPalletDto
    {
        public string Name { get; set; } = "";
        public int Width { get; set; }
        public int Length { get; set; }
        public int Weight { get; set; }
        public int Height { get; set; }

        // accept string enum values (e.g. "Plastic") as well as numeric enum values
        [JsonConverter(typeof(JsonStringEnumConverter))]
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
