﻿using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RazorLogin.Models;

namespace RazorLogin.Pages.CustomerShop
{
    public class IndexModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(RazorLogin.Models.ZooDbContext context, IHttpContextAccessor httpContextAccessor, ILogger<IndexModel> logger)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public IList<Item> Item { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Item = await _context.Items.ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync(int itemId, int quantity)
        {
            // Get the logged-in user's email
            var userEmail = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(userEmail))
            {
                _logger.LogWarning("User email is null or empty.");
                return Page();
            }

            // Retrieve customer by email
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerEmail == userEmail);
            if (customer == null)
            {
                _logger.LogWarning($"No customer found for email: {userEmail}");
                return Page();
            }

            // Find the item being purchased
            var item = await _context.Items.FindAsync(itemId);
            if (item == null || item.ItemCount < quantity)
            {
                ModelState.AddModelError("", "Insufficient stock or item not found.");
                return Page();
            }

            // Update the stock of the item
            item.ItemCount -= quantity;
            _context.Items.Update(item);

            // Generate a unique PurchaseId using a random number
            Random random = new Random();
            int purchaseId;
            do
            {
                purchaseId = random.Next(10000, 99999);
            } while (await _context.Purchases.AnyAsync(p => p.PurchaseId == purchaseId));

            
            var purchase = new Purchase
            {
                PurchaseId = purchaseId, 
                CustomerId = customer.CustomerId,
                PurchaseDate = DateOnly.FromDateTime(DateTime.Now), 
                PurchaseTime = TimeOnly.FromDateTime(DateTime.Now), 
                NumItems = quantity,
                StoreId = item.ShopId
            };

            // Save the new purchase
            _context.Purchases.Add(purchase);
            await _context.SaveChangesAsync();

            // Redirect to refresh the page after the purchase
            return RedirectToPage("./Index");
        }
    }
}
