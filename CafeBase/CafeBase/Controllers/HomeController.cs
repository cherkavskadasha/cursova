using CafeBase.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CafeBase.Controllers
{
    public class HomeController : Controller
    {
        private readonly CafeBaseContext _context;

        public HomeController(CafeBaseContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public async Task<IActionResult> OrderDay()
        {
            var events = _context.Orders.FromSqlRaw("SELECT * FROM Orders WHERE CONVERT(date, OrderDate) = CONVERT(date, GETDATE())").ToList();
            return View(events);
        }

        public async Task<IActionResult> CustomerDay()
        {
            var events = _context.Customers.FromSqlRaw("SELECT * FROM Customers WHERE CONVERT(date, RegistrationDate) = CONVERT(date, GETDATE())").ToList();
            return View(events);
        }

        public async Task<IActionResult> PaymentDay()
        {
            var events = _context.Payments.FromSqlRaw("SELECT *, (SELECT SUM(AmountPaid)  FROM Payments  WHERE CONVERT(date, PaymentDate) = CONVERT(date, GETDATE())) AS TotalAmountPaid FROM Payments WHERE CONVERT(date, PaymentDate) = CONVERT(date, GETDATE());").ToList();
            return View(events);
        }
    }
}
