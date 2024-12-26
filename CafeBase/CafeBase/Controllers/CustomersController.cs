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
            if (string.IsNullOrEmpty(search.SearchTerm) && string.IsNullOrEmpty(search.Phone) && string.IsNullOrEmpty(search.Email) && string.IsNullOrEmpty(search.RegistrationDate))
            {
                return View(await _context.Customers.ToListAsync());
            }
            else
            {
                var sqlQuery = new StringBuilder("SELECT * FROM Customers WHERE 1=1");
                var sqlParameters = new List<SqlParameter>();

                if (!string.IsNullOrEmpty(search?.SearchTerm))
                {
                    sqlQuery.Append(" AND Name LIKE @Name");
                    sqlParameters.Add(new SqlParameter("@Name", "%" + search?.SearchTerm + "%"));
                }

                if (!string.IsNullOrEmpty(search?.Phone))
                {
                    sqlQuery.Append(" AND Phone LIKE @Phone");
                    sqlParameters.Add(new SqlParameter("@Phone", "%" + search?.Phone + "%"));
                }

                if (!string.IsNullOrEmpty(search?.Email))
                {
                    sqlQuery.Append(" AND Email LIKE @Email");
                    sqlParameters.Add(new SqlParameter("@Email", "%" + search?.Email + "%"));
                }

                if (search?.RegistrationDate != null)
                {
                    sqlQuery.Append(" AND RegistrationDate = @RegistrationDate");
                    sqlParameters.Add(new SqlParameter("@RegistrationDate", search?.RegistrationDate));
                }

                var events = _context.Customers
                    .FromSqlRaw(sqlQuery.ToString(), sqlParameters.ToArray())
                    .ToList();

                return View(events);
            }
        }
        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerId,Name,Phone,Email,RegistrationDate")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CustomerId,Name,Phone,Email,RegistrationDate")] Customer customer)
        {
            if (id != customer.CustomerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.CustomerId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
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
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
        }
    }
}
