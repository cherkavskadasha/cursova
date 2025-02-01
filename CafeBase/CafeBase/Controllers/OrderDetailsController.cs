using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CafeBase.Models;

namespace CafeBase.Controllers
{
    public class OrderDetailsController : Controller
    {
        private readonly CafeBaseContext _context;

        public OrderDetailsController(CafeBaseContext context)
        {
            _context = context;
        }

        // GET: OrderDetails
        public async Task<IActionResult> Index()
        {
            var cafeBaseContext = _context.OrderDetails.Include(o => o.Item).Include(o => o.Order);
            return View(await cafeBaseContext.ToListAsync());
        }

        // GET: OrderDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var orderDetail = await GetOrderDetailById(id, true);
            if (orderDetail == null) return NotFound();
            return View(orderDetail);
        }

        // GET: OrderDetails/Create
        public IActionResult Create()
        {
            PopulateSelectLists();
            return View();
        }

        // POST: OrderDetails/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind(nameof(OrderDetail.OrderDetailId), nameof(OrderDetail.OrderId), nameof(OrderDetail.ItemId), nameof(OrderDetail.Quantity), nameof(OrderDetail.Subtotal))] OrderDetail orderDetail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(orderDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            PopulateSelectLists(orderDetail);
            return View(orderDetail);
        }

        // GET: OrderDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var orderDetail = await GetOrderDetailById(id);
            if (orderDetail == null) return NotFound();

            PopulateSelectLists(orderDetail);
            return View(orderDetail);
        }

        // POST: OrderDetails/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind(nameof(OrderDetail.OrderDetailId), nameof(OrderDetail.OrderId), nameof(OrderDetail.ItemId), nameof(OrderDetail.Quantity), nameof(OrderDetail.Subtotal))] OrderDetail orderDetail)
        {
            if (id != orderDetail.OrderDetailId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderDetailExists(orderDetail.OrderDetailId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            PopulateSelectLists(orderDetail);
            return View(orderDetail);
        }

        // GET: OrderDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var orderDetail = await GetOrderDetailById(id, true);
            if (orderDetail == null) return NotFound();
            return View(orderDetail);
        }

        // POST: OrderDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var orderDetail = await _context.OrderDetails.FindAsync(id);
                if (orderDetail != null)
                {
                    _context.OrderDetails.Remove(orderDetail);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while deleting the record.");
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

        private bool OrderDetailExists(int id)
        {
            return _context.OrderDetails.Any(e => e.OrderDetailId == id);
        }

        private async Task<OrderDetail?> GetOrderDetailById(int? id, bool includeNavigation = false)
        {
            if (id == null) return null;

            IQueryable<OrderDetail> query = _context.OrderDetails;
            if (includeNavigation) query = query.Include(o => o.Item).Include(o => o.Order);

            return await query.FirstOrDefaultAsync(m => m.OrderDetailId == id);
        }

        private void PopulateSelectLists(OrderDetail? orderDetail = null)
        {
            ViewData[nameof(OrderDetail.ItemId)] = new SelectList(_context.MenuItems, nameof(MenuItem.ItemId), nameof(MenuItem.ItemId), orderDetail?.ItemId);
            ViewData[nameof(OrderDetail.OrderId)] = new SelectList(_context.Orders, nameof(Order.OrderId), nameof(Order.OrderId), orderDetail?.OrderId);
        }
    }

}
