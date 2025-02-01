using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CafeBase.Models;
using Microsoft.Data.SqlClient;
using System.Text;

namespace CafeBase.Controllers
{
    public class CustomersController : Controller
    {
        private readonly CafeBaseContext _context;

        public CustomersController(CafeBaseContext context)
        {
            _context = context;
        }

        // GET: Customers
        public async Task<IActionResult> Index(CustomersSearch search)
        {
            IQueryable<Customer> query = _context.Customers;

            if (!string.IsNullOrEmpty(search?.SearchTerm))
            {
                query = query.Where(c => c.Name.Contains(search.SearchTerm));
            }

            if (!string.IsNullOrEmpty(search?.Phone))
            {
                query = query.Where(c => c.Phone.Contains(search.Phone));
            }

            if (!string.IsNullOrEmpty(search?.Email))
            {
                query = query.Where(c => c.Email.Contains(search.Email));
            }

            if (!string.IsNullOrEmpty(search?.RegistrationDate) && DateTime.TryParse(search.RegistrationDate, out DateTime date))
            {
                DateOnly dateOnly = DateOnly.FromDateTime(date); 
                query = query.Where(c => c.RegistrationDate == dateOnly);
            }

            return View(await query.ToListAsync());
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            return customer == null ? NotFound() : View(customer);
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerId,Name,Phone,Email,RegistrationDate")] Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return View(customer);
            }

            _context.Add(customer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            return customer == null ? NotFound() : View(customer);
        }

        // POST: Customers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CustomerId,Name,Phone,Email,RegistrationDate")] Customer customer)
        {
            if (id != customer.CustomerId || !ModelState.IsValid)
            {
                return View(customer);
            }

            _context.Update(customer);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!CustomerExists(customer.CustomerId))
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            return customer == null ? NotFound() : View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(int id) => _context.Customers.Any(e => e.CustomerId == id);
    }

}
