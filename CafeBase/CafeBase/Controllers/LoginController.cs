using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CafeBase.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var users = new[]
            {
            new { Username = "admin", Password = "1111"}
            };

            var user = users.FirstOrDefault(u => u.Username == username && u.Password == password);
            if (user != null)
            {
                await SignInUser(user.Username, "admini");
                return RedirectToAction("Login", "Login");
            }

            ViewBag.ErrorMessage = "Invalid login credentials.";
            return View();
        }
        public async Task<IActionResult> NOLogin()
        {
            await SignInUser("name", "customer");
            return RedirectToAction("Login", "Login");
        }

        private async Task SignInUser(string username, string role)
        {
            var claims = new List<Claim>
     {
         new Claim(ClaimTypes.Name, username),
         new Claim(ClaimTypes.Role, role)
     };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties();

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }
    }
}
