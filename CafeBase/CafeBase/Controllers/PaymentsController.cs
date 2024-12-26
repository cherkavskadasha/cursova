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
    public class PaymentsController : Controller
    {
        private readonly CafeBaseContext _context;

        public PaymentsController(CafeBaseContext context)
        {
            _context = context;
        }

        // GET: Payments
        public async Task<IActionResult> Index(PaymentsSearch search)
        {
            if (string.IsNullOrEmpty(search.AmountPaid) && string.IsNullOrEmpty(search.PaymentDate) && string.IsNullOrEmpty(search.Order))
            {
                return View(await _context.Payments.Include(o => o.Order).ToListAsync());
            }
            else
            {
                var sqlQuery = new StringBuilder("SELECT * FROM Payments WHERE 1=1");
                var sqlParameters = new List<SqlParameter>();

                if (!string.IsNullOrEmpty(search?.AmountPaid))
                {
                    sqlQuery.Append(" AND AmountPaid = @AmountPaid");
                    sqlParameters.Add(new SqlParameter("@AmountPaid", search?.AmountPaid));
                }

                if (!string.IsNullOrEmpty(search?.PaymentDate))
                {
                    if (DateTime.TryParse(search.PaymentDate, out var paymentDate))
                    {
                        sqlQuery.Append(" AND CAST(PaymentDate AS DATE) = @PaymentDate");
                        sqlParameters.Add(new SqlParameter("@PaymentDate", paymentDate.Date)); // Передаємо лише дату, без часу
                    }
                    else
                    {
                        ModelState.AddModelError("PaymentDate", "Неправильний формат дати");
                    }
                }

                if (!string.IsNullOrEmpty(search?.Order))
                {
                    sqlQuery.Append(" AND OrderId = @OrderId");
                    sqlParameters.Add(new SqlParameter("@OrderId", search?.Order));
                }

                var payments = _context.Payments
                    .FromSqlRaw(sqlQuery.ToString(), sqlParameters.ToArray())
                    .Include(o => o.Order) // Load related orders
                    .ToList();

                return View(payments);
            }

        }
        // GET: Payments/Details/5
        public async Task<IActionResult> Details(int? id)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var payment = await _context.Payments
                    .Include(p => p.Order)
                    .FirstOrDefaultAsync(m => m.PaymentId == id);
                if (payment == null)
                {
                    return NotFound();
                }

                return View(payment);
            }

            // GET: Payments/Create
            public IActionResult Create()
            {
                ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "OrderId");
                return View();
            }

            // POST: Payments/Create
            // To protect from overposting attacks, enable the specific properties you want to bind to.
            // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Create([Bind("PaymentId,OrderId,PaymentDate,AmountPaid,PaymentMethod")] Payment payment)
            {
                    _context.Add(payment);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
            }

            // GET: Payments/Edit/5
            public async Task<IActionResult> Edit(int? id)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var payment = await _context.Payments.FindAsync(id);
                if (payment == null)
                {
                    return NotFound();
                }
                ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "OrderId", payment.OrderId);
                return View(payment);
            }

            // POST: Payments/Edit/5
            // To protect from overposting attacks, enable the specific properties you want to bind to.
            // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Edit(int id, [Bind("PaymentId,OrderId,PaymentDate,AmountPaid,PaymentMethod")] Payment payment)
            {
                if (id != payment.PaymentId)
                {
                    return NotFound();
                }
                    try
                    {
                        _context.Update(payment);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!PaymentExists(payment.PaymentId))
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

            // GET: Payments/Delete/5
            public async Task<IActionResult> Delete(int? id)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var payment = await _context.Payments
                    .Include(p => p.Order)
                    .FirstOrDefaultAsync(m => m.PaymentId == id);
                if (payment == null)
                {
                    return NotFound();
                }

                return View(payment);
            }

            // POST: Payments/Delete/5
            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> DeleteConfirmed(int id)
            {
                var payment = await _context.Payments.FindAsync(id);
                if (payment != null)
                {
                    _context.Payments.Remove(payment);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            private bool PaymentExists(int id)
            {
                return _context.Payments.Any(e => e.PaymentId == id);
            }
        }
    }