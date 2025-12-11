using Microsoft.AspNetCore.Mvc;
using Pallet_Optimizer.Data;
using Pallet_Optimizer.Filters;
using System.Threading.Tasks;

namespace Pallet_Optimizer.Controllers
{
    [AuthorizeSession]
    public class DashboardController : Controller
    {
        private readonly IPalletRepository _repo;

        public DashboardController(IPalletRepository repo)
        {
            _repo = repo;
        }

        // GET: /Dashboard
        public async Task<IActionResult> Index()
        {
            var packagePlans = await _repo.GetAllPackagePlansAsync();
            return View(packagePlans);
        }

        // POST: /Dashboard/CreatePlan
        [HttpPost]
        public async Task<IActionResult> CreatePlan(string planName)
        {
            if (string.IsNullOrWhiteSpace(planName))
            {
                TempData["Error"] = "Pakkeplan navn er påkrævet";
                return RedirectToAction(nameof(Index));
            }

            var planId = await _repo.CreatePackagePlanAsync(planName);
            return RedirectToAction("Index", "Pallet", new { planId });
        }

        // POST: /Dashboard/DeletePlan
        [HttpPost]
        public async Task<IActionResult> DeletePlan(string id)
        {
            var success = await _repo.DeletePackagePlanAsync(id);

            if (success)
            {
                TempData["Success"] = "Pakkeplan blev slettet";
            }
            else
            {
                TempData["Error"] = "Kunne ikke slette pakkeplan";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /Dashboard/EditPlan
        public IActionResult EditPlan(string id)
        {
            return RedirectToAction("Index", "Pallet", new { planId = id });
        }

        // GET: /Dashboard/Settings
        public IActionResult Settings()
        {
            return View();
        }
    }
}