using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StallMate.Data;
using StallMate.Models;

namespace StallMate.Controllers
{
    [Authorize]
    public class InventoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public InventoryController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Show all items for the logged in user
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var items = await _context.Items
                .Where(i => i.UserId == userId)
                .OrderByDescending(i => i.DateAdded)
                .ToListAsync();
            return View(items);
        }

        // Show form to add new item
        public IActionResult Create()
        {
            return View();
        }

        // Save new item
        [HttpPost]
        public async Task<IActionResult> Create(Item item)
        {
            if (ModelState.IsValid)
            {
                item.UserId = _userManager.GetUserId(User) ?? "";
                item.DateAdded = DateTime.Now;
                _context.Items.Add(item);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(item);
        }

        // Mark item as sold
        [HttpPost]
        public async Task<IActionResult> MarkAsSold(int id, decimal salePrice)
        {
            var userId = _userManager.GetUserId(User);
            var item = await _context.Items
                .FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId);
            if (item != null)
            {
                item.IsSold = true;
                item.SalePrice = salePrice;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
        // Delete item
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User);
            var item = await _context.Items
                .FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId);
            if (item != null)
            {
                _context.Items.Remove(item);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
        // Show edit form
        public async Task<IActionResult> Edit(int id)
        {
            var userId = _userManager.GetUserId(User);
            var item = await _context.Items
                .FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId);
            if (item == null) return NotFound();
            return View(item);
        }

        // Save edited item
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Item item)
        {
            var userId = _userManager.GetUserId(User);
            var existing = await _context.Items
                .FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId);
            if (existing == null) return NotFound();

            if (ModelState.IsValid)
            {
                existing.Name = item.Name;
                existing.Description = item.Description;
                existing.PurchasePrice = item.PurchasePrice;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(item);
        }
        // Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var userId = _userManager.GetUserId(User);
            var items = await _context.Items
                .Where(i => i.UserId == userId)
                .ToListAsync();

            ViewBag.TotalItems = items.Count;
            ViewBag.AvailableItems = items.Count(i => !i.IsSold);
            ViewBag.SoldItems = items.Count(i => i.IsSold);
            ViewBag.TotalSpent = items.Sum(i => i.PurchasePrice);
            ViewBag.TotalEarned = items.Where(i => i.IsSold).Sum(i => i.SalePrice ?? 0);
            ViewBag.TotalProfit = ViewBag.TotalEarned - ViewBag.TotalSpent;

            return View();
        }
    }
}