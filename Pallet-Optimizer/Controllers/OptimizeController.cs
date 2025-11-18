using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Pallet_Optimizer.Controllers
{
    public class OptimizeController : Controller
    {
        // GET: OptimizeController
        public ActionResult Index()
        {
            return View("Optimize");
        }

        // GET: OptimizeController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: OptimizeController/Create
        public ActionResult Create()
        {
            return View();
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

        // GET: OptimizeController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: OptimizeController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction();
            }
            catch
            {
                return View();
            }
        }

        // GET: OptimizeController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: OptimizeController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
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
