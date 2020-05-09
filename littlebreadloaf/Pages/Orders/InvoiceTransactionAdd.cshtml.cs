using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using littlebreadloaf.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace littlebreadloaf.Pages.Orders
{
    [Authorize]
    public class InvoiceTransactionAddModel : PageModel
    {
        private readonly ProductContext _context;
        public InvoiceTransactionAddModel(ProductContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string OrderID { get; set; }

        [BindProperty]
        public InvoiceTransaction InvoiceTransaction { get; set; }

        [BindProperty]
        public IEnumerable<SelectListItem> TransactionCategories { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool AddPayment { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool AddDiscount { get; set; }

        [BindProperty(SupportsGet = true)]
        public string DiscountType { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (String.IsNullOrEmpty(OrderID) || !Guid.TryParse(OrderID, out Guid parsedID))
            {
                return new RedirectResult("/Orders/OrdersList");
            }

            TransactionCategories = new SelectList(new List<SelectListItem>()
            {
                new SelectListItem(){ Text = InvoiceHelper.Transaction_Category_Discount, Value = InvoiceHelper.Transaction_Category_Discount, Selected = false },
                new SelectListItem(){ Text = InvoiceHelper.Transaction_Category_Payment, Value = InvoiceHelper.Transaction_Category_Payment, Selected = false }
            }, "Text", "Value", null);
            

            if (!await _context.ProductOrder.AnyAsync(p => p.OrderID == parsedID))
            {
                return new RedirectResult("/Orders/OrdersList");
            }

            var invoice = await _context.Invoice.FirstOrDefaultAsync(f => f.ProductOrderID == parsedID);
            if (invoice == null)
            {
                return new RedirectResult("/Orders/OrdersList");
            }

            var total = _context.InvoiceTransaction.Where(w => w.InvoiceID == invoice.InvoiceID).Sum(s => s.Quantity * s.Price);

            if (AddPayment)
                InvoiceTransaction = new InvoiceTransaction()
                {
                    InvoiceID = invoice.InvoiceID,
                    Type = InvoiceHelper.Transaction_Type_Credit,
                    Category = InvoiceHelper.Transaction_Category_Payment,
                    Price = -1 * total,
                    Quantity = 1,
                    Name = "Payment",
                    Description = "Payment in full"
                };

            if (AddDiscount && !string.IsNullOrEmpty(DiscountType))
            {
                DiscountRate(out decimal rate, out string description);
                InvoiceTransaction = new InvoiceTransaction()
                {
                    InvoiceID = invoice.InvoiceID,
                    Type = InvoiceHelper.Transaction_Type_Credit,
                    Category = InvoiceHelper.Transaction_Category_Discount,
                    Price = (-1 * total) * rate,
                    Quantity = 1,
                    Name = "Discount",
                    Description = description
                };
            }
                
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                return Page();
            }

            if (String.IsNullOrEmpty(OrderID) || !Guid.TryParse(OrderID, out Guid parsedID))
            {
                return new RedirectResult("/Orders/OrdersList");
            }

            var order = await _context.ProductOrder.FirstOrDefaultAsync(f => f.OrderID == parsedID);
            if(order == null)
                return new RedirectResult("/Orders/OrdersList");

            var invoice = await _context.Invoice.FirstOrDefaultAsync(f => f.ProductOrderID == parsedID);
            var balance = await _context.InvoiceTransaction
                                        .Where(w => w.InvoiceID == invoice.InvoiceID)
                                        .SumAsync(s => s.Quantity * s.Price);

            if(balance == 0) //Check existing balance amount without taking into effect the new transaction amount
            {
                ModelState.AddModelError("BalanceZero", "Cannot add transaction. The order balance is already zero.");
                return Page();
            }

            balance += InvoiceTransaction.Price;
           
            if(balance < 0)
            {
                ModelState.AddModelError("BalanceCredit", "Cannot add transaction. The balance cannot be less than zero.");
                return Page();
            }

            if (balance == 0)
            {
                order.Payment = DateTime.Now;
                _context.ProductOrder.Update(order);
            }
            await _context.InvoiceTransaction.AddAsync(InvoiceTransaction);
            await _context.SaveChangesAsync();
            return new RedirectToPageResult("/Orders/InvoiceView", new { OrderID = parsedID });
        }


        private void DiscountRate(out decimal rate, out string description)
        {
            DiscountType = DiscountType.ToUpper();
            if (DiscountType == "5P")
            {
                rate = 0.05M;
                description = "5% discount";
                return;
            }
                
            if (DiscountType == "10P")
            {
                rate = 0.10M;
                description = "10% discount";
                return;
            }
                
            if (DiscountType == "15P")
            {
                rate = 0.15M;
                description = "15% discount";
                return;
            }
               
            if (DiscountType == "20P")
            {
                rate = 0.20M;
                description = "20% discount";
                return;
            }

            description  = "Discount";
            rate = 0;
        }
    }
}