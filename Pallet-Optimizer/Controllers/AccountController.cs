using Microsoft.AspNetCore.Mvc;
using Pallet_Optimizer.Models;

namespace Pallet_Optimizer.Controllers
{
    public class AccountController : Controller
    {
        // Hardcoded brugere - du kan senere flytte dette til en database
        private readonly List<User> _users = new()
        {
            new User { Username = "admin", Password = "admin123" },
            new User { Username = "user", Password = "user123" }
        };

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = _users.FirstOrDefault(u =>
                u.Username == model.Username && u.Password == model.Password);

            if (user != null)
            {
                HttpContext.Session.SetString("Username", user.Username);
                return RedirectToAction("Index", "Pallet");
            }

            ModelState.AddModelError("", "Ugyldigt brugernavn eller password");
            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}