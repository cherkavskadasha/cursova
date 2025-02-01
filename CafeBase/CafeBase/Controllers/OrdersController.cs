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
using Microsoft.IdentityModel.Tokens;

namespace CafeBase.Controllers
{
    public class OrdersController : Controller
    {
        private readonly CafeBaseContext _context;

        public OrdersController(CafeBaseContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(OrdersSearch search)
        {
            if (string.IsNullOrEmpty(search.OrderDate) && string.IsNullOrEmpty(search.Customer) && string.IsNullOrEmpty(search.TotalAmount))
            {
                return View(await _context.Orders.Include(o => o.Customer).ToListAsync());
            }
            else
            {
                var sqlQuery = new StringBuilder("SELECT * FROM Orders WHERE 1=1");
                var sqlParameters = new List<SqlParameter>();

                if (!string.IsNullOrEmpty(search?.OrderDate))
                {
                    if (DateTime.TryParse(search.OrderDate, out var orderDate))
                    {
                        sqlQuery.Append(" AND CAST(OrderDate AS DATE) = @OrderDate");
                        sqlParameters.Add(new SqlParameter("@OrderDate", orderDate.Date)); 
                    }
                    else
                    {
                        ModelState.AddModelError("PaymentDate", "Неправильний формат дати");
                    }
                }

                if (!string.IsNullOrEmpty(search?.Customer))
                {
                    sqlQuery.Append(" AND CustomerId IN (SELECT CustomerId FROM Customers WHERE Name LIKE @Customer)");
                    sqlParameters.Add(new SqlParameter("@Customer", "%" + search?.Customer + "%"));
                }

                if (!string.IsNullOrEmpty(search?.TotalAmount))
                {
                    sqlQuery.Append(" AND TotalAmount = @TotalAmount");
                    sqlParameters.Add(new SqlParameter("@TotalAmount", search?.TotalAmount));
                }

                var orders = _context.Orders
                    .FromSqlRaw(sqlQuery.ToString(), sqlParameters.ToArray())
                    .Include(o => o.Customer) 
                    .ToList();

                return View(orders);
            }
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "Name");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,CustomerId,OrderDate,TotalAmount")] Order order)
        {
                _context.Add(order);
                await _context.SaveChangesAsync();

            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "Name", order.CustomerId);
            return RedirectToAction(nameof(Index));
            
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "Name", order.CustomerId);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,CustomerId,OrderDate,TotalAmount")] Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }

                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
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

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}
