using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Pallet_Optimizer.Models;

namespace Pallet_Optimizer.Controllers
{
    public class OptimizeController : Controller
    {
        // GET: OptimizeController
        public ActionResult Index()
        {

            return View("Optimize", new PalletHolder());
        }



        // GET: OptimizeController/Create
        public ActionResult Create()
        {
            return View();
        }

        public ActionResult OnPalletTypeChange(List<Pallet> pallets)
        {
            Console.WriteLine("pallet changed");
            foreach(var pallet in pallets)
            {
                // Perform some logic based on pallet properties
                if (pallet.Active)
                {
                    // Example logic: Log active pallets
                    Console.WriteLine($"Active Pallet: {pallet.Description}");
                }
            }

            return View("Optimize", pallets);
        }

        // POST: OptimizeController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

    }
}
