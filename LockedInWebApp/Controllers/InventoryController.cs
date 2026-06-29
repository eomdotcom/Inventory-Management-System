using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LockedInWebApp.Data;
using LockedInWebApp.Models;

namespace LockedInWebApp.Controllers
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
    }
}