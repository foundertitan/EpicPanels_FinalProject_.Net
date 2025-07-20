using Microsoft.AspNetCore.Mvc;
using EpicPanels_Group5_FinalProject.Models;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;
using EpicPanels_Group5_FinalProject.Data;
using Microsoft.EntityFrameworkCore;

namespace EpicPanels_Group5_FinalProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string Email, string Password)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == Email && u.Password == Password);

                if (user != null)
                {
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

                    var identity = new ClaimsIdentity(claims, "Cookies");
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync("Cookies", principal);

                    if (user.Role?.ToLower() == "admin")
                    {
                        return RedirectToAction("Index", "Admin");
                    }

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Invalid email or password.");
            }
            return View();
        }


        [HttpGet]
        public IActionResult Signup() => View();

        [HttpPost]
        public async Task<IActionResult> Signup(User user, string ConfirmPassword)
        {
            if (ModelState.IsValid && user.Password == ConfirmPassword)
            {
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == user.Email);

                if (existingUser != null)
                {
                    ModelState.AddModelError("", "Email is already registered.");
                    return View(user);
                }
                user.Role = "Customer";

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return RedirectToAction("Login");
            }

            ModelState.AddModelError("", "Passwords do not match.");
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Cookies");
            return RedirectToAction("Login");
        }
    }
}
