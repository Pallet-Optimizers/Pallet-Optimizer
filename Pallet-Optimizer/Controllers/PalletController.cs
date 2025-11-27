using Microsoft.AspNetCore.Mvc;
using Pallet_Optimizer.Data;
using Pallet_Optimizer.Models;
using Pallet_Optimizer.Algorithms;
using System.Linq;
using System.Threading.Tasks;
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

        public IActionResult Index()
        {
            return View();
        }


        // ---------------------------------------------------------------
        // GET HOLDER
        // ---------------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetPalletHolder()
        {
            var holder = await _repo.GetHolderAsync();
            return Json(holder);
        }

        // ---------------------------------------------------------------
        // ADD PALLET
        // ---------------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> AddPallet([FromBody] Pallet dto)
        {
            if (dto == null) return BadRequest();

            dto.Id = System.Guid.NewGuid().ToString();
            dto.Elements = new List<Element>();

            await _repo.AddPalletAsync(dto);

            var holder = await _repo.GetHolderAsync();
            return Json(new { success = true, holder });
        }

        // ---------------------------------------------------------------
        // DELETE PALLET
        // ---------------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> DeletePallet([FromBody] DeletePalletDto dto)
        {
            if (dto == null) return BadRequest();

            await _repo.DeletePalletAsync(dto.PalletId);

            var holder = await _repo.GetHolderAsync();
            return Json(new { success = true, holder });
        }

        // ---------------------------------------------------------------
        // UPDATE PALLET METADATA
        // ---------------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> UpdatePallet([FromBody] UpdatePalletDto dto)
        {
            if (dto == null) return BadRequest();

            var pallet = await _repo.GetPalletAsync(dto.PalletId);
            if (pallet == null) return NotFound();

            pallet.Name = dto.Name ?? pallet.Name;
            pallet.MaterialType = dto.MaterialType ?? pallet.MaterialType;
            pallet.Width = dto.Width ?? pallet.Width;
            pallet.Length = dto.Length ?? pallet.Length;
            pallet.MaxHeight = dto.MaxHeight ?? pallet.MaxHeight;
            pallet.MaxWeightKg = dto.MaxWeightKg ?? pallet.MaxWeightKg;

            await _repo.UpdatePalletAsync(pallet.Id, pallet);

            return Json(new { success = true, pallet });
        }

        // ---------------------------------------------------------------
        // ADD ELEMENT → REPACK ALL PALLETS
        // ---------------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> AddElement([FromBody] AddElementDto dto)
        {
            if (dto == null) return BadRequest();

            var holder = await _repo.GetHolderAsync();

            if (holder.Pallets == null || holder.Pallets.Count == 0)
            {
                var defaultPallet = new Pallet
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Auto pallet",
                    Width = 1.2,
                    Length = 0.8,
                    MaxHeight = 1.8,
                    MaxWeightKg = 1000,
                    Elements = new List<Element>()
                };

                holder.Pallets = new List<Pallet> { defaultPallet };
                await _repo.UpdateHolderAsync(holder);
            }


            // collect all existing elements
            var all = holder.Pallets
                .SelectMany(p => p.Elements)
                .Select(CloneElementForAlgorithm)
                .ToList();

            // add incoming element
            var newElement = new Element
            {
                Id = System.Guid.NewGuid().ToString(),
                Name = dto.Name,
                Width = dto.Width,
                Depth = dto.Depth,
                Height = dto.Height,
                WeightKg = dto.WeightKg,
                CanRotate = dto.CanRotate,
                MustBeAlone = dto.MustBeAlone
            };

            all.Add(newElement);

            // repack with greedy algorithm
            var settings = new PackingSettings
            {
                Padding = dto.Padding ?? 0.02,
                AllowRotation = dto.AllowRotation ?? true,
                MaxOverhangX = dto.MaxOverhangX ?? 0.0,
                MaxOverhangY = dto.MaxOverhangY ?? 0.0,
                RespectMustBeAlone = true,
                MaxPalletHeightAbsolute = holder.Pallets.First().MaxHeight
            };

            // create a template pallet from the first existing pallet
            var template = ClonePalletTemplate(holder.Pallets.First());

            // create a dummy holder containing only the template
            var dummy = new PalletHolder { Pallets = new List<Pallet> { template } };

            // attach all elements to template so algorithm can extract them
            template.Elements = all.Select(CloneElementForAlgorithm).ToList();

            // run the algorithm
            var packedHolder = GreedyPalletOptimizer.PackAll(dummy, settings);

            // save result
            await _repo.UpdateHolderAsync(packedHolder);

            return Json(new { success = true, holder = packedHolder });
        }

        // ---------------------------------------------------------------
        // REMOVE ELEMENT → REPACK ALL PALLETS
        // ---------------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> RemoveElement([FromBody] RemoveElementDto dto)
        {
            if (dto == null) return BadRequest();

            var holder = await _repo.GetHolderAsync();

            // remove element everywhere
            foreach (var pallet in holder.Pallets)
                pallet.Elements.RemoveAll(e => e.Id == dto.ElementId);

            // collect remaining
            var all = holder.Pallets
                .SelectMany(p => p.Elements)
                .Select(CloneElementForAlgorithm)
                .ToList();

            var template = ClonePalletTemplate(holder.Pallets.First());
            var dummy = new PalletHolder { Pallets = new List<Pallet> { template } };
            template.Elements = all.Select(CloneElementForAlgorithm).ToList();

            var settings = new PackingSettings();

            var packedHolder = GreedyPalletOptimizer.PackAll(dummy, settings);

            await _repo.UpdateHolderAsync(packedHolder);

            return Json(new { success = true, holder = packedHolder });
        }

        // ---------------------------------------------------------------
        // DTOs + HELPERS
        // ---------------------------------------------------------------

        private static Element CloneElementForAlgorithm(Element e)
        {
            return new Element
            {
                Id = e.Id,
                Name = e.Name,
                Width = e.Width,
                Depth = e.Depth,
                Height = e.Height,
                WeightKg = e.WeightKg,
                CanRotate = e.CanRotate,
                MustBeAlone = e.MustBeAlone
            };
        }

        private static Pallet ClonePalletTemplate(Pallet p)
        {
            return new Pallet
            {
                Id = System.Guid.NewGuid().ToString(),
                Name = p.Name,
                MaterialType = p.MaterialType,
                Width = p.Width,
                Length = p.Length,
                MaxHeight = p.MaxHeight,
                MaxWeightKg = p.MaxWeightKg,
                Elements = new List<Element>()
            };
        }
    }

    // ===================================================================
    // DTO DEFINITIONS
    // ===================================================================
    public class AddElementDto
    {
        public string Name { get; set; }
        public double Width { get; set; }
        public double Depth { get; set; }
        public double Height { get; set; }
        public double WeightKg { get; set; }
        public bool CanRotate { get; set; } = true;
        public bool MustBeAlone { get; set; } = false;

        public double? Padding { get; set; }
        public bool? AllowRotation { get; set; }
        public double? MaxOverhangX { get; set; }
        public double? MaxOverhangY { get; set; }
    }

    public class RemoveElementDto
    {
        public string ElementId { get; set; }
    }

    public class DeletePalletDto
    {
        public string PalletId { get; set; }
    }

    public class UpdatePalletDto
    {
        public string PalletId { get; set; }
        public string Name { get; set; }
        public PALLET_MATERIAL_TYPE? MaterialType { get; set; }
        public double? Width { get; set; }
        public double? Length { get; set; }
        public double? MaxHeight { get; set; }
        public double? MaxWeightKg { get; set; }
        public double? WeightKg { get; set; }
    }
}
