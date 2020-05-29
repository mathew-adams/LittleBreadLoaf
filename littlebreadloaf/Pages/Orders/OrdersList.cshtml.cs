using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using littlebreadloaf.Data;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.IO;
using ClosedXML.Excel;

namespace littlebreadloaf.Pages.Orders
{
    [Authorize]
    public class OrdersListModel : PageModel
    {

        private const string excelName = "Orders";

        private readonly ProductContext _context;

        public OrdersListModel(ProductContext context)
        {
            _context = context;
        }

        [BindProperty]
        public List<ProductOrder> ProductOrders { get; set; }

        [BindProperty(SupportsGet = true)]
        public string FilterConfirmationCode { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? FilterDeliveryDateFrom { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? FilterDeliveryDateTo { get; set; }

        [BindProperty(SupportsGet = true)]
        public string FilterEmail { get; set; }

        [BindProperty(SupportsGet = true)]
        public string FilterPhone { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool FilterShowAll { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var query = (from orders in _context.ProductOrder select orders);

            query = query.Where(w => w.ConfirmationCode != "");

            if (!FilterShowAll)
                query = query.Where(w => w.Payment == new DateTime(9999,12,31));

            if (!string.IsNullOrEmpty(FilterConfirmationCode))
                query = query.Where(w => w.ConfirmationCode.Contains(FilterConfirmationCode, StringComparison.OrdinalIgnoreCase));

            if (FilterDeliveryDateFrom != null)
                query = query.Where(df => df.DeliveryDate >= FilterDeliveryDateFrom);

            if (FilterDeliveryDateTo != null)
                query = query.Where(dt => dt.DeliveryDate <= FilterDeliveryDateTo);

            if (!string.IsNullOrEmpty(FilterEmail))
                query = query.Where(e => e.ContactEmail.Contains(FilterEmail, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(FilterPhone))
                query = query.Where(e => e.ContactPhone.Contains(FilterPhone, StringComparison.OrdinalIgnoreCase));

            ProductOrders = await query.ToListAsync();

            ProductOrders = ProductOrders.OrderByDescending(o => o.Created).ToList();

            return Page();
        }

        public async Task<ActionResult> OnPostExportExcelAsync(bool showAll)
        {
            List<ExcelOrders> orders;
            if(showAll)
            {
                orders = await OrdersHelper.GetAllOrders(_context);
            }
            else
            {
                orders = await OrdersHelper.GetActiveOrders(_context);
            }

            var dtCustomers = new DataTable();
            dtCustomers.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("Created", typeof(DateTime)),
                new DataColumn("Confirmed", typeof(DateTime)),
                new DataColumn("DeliveryDate", typeof(DateTime)),
                new DataColumn("DeliveryTime", typeof(string)),
                new DataColumn("PickupDate", typeof(DateTime)),
                new DataColumn("PickupTime", typeof(string)),
                new DataColumn("ContactName", typeof(string)),
                new DataColumn("ContactEmail", typeof(string)),
                new DataColumn("ContactPhone", typeof(string)),
                new DataColumn("ConfirmationCode", typeof(string)),
                new DataColumn("Address", typeof(string)),
                new DataColumn("Suburb", typeof(string)),
                new DataColumn("Town", typeof(string)),
                new DataColumn("Name", typeof(string)),
                new DataColumn("Quantity", typeof(Int32)),
                new DataColumn("Price", typeof(Decimal)),
                new DataColumn("ProductSum", typeof(Decimal)),
                new DataColumn("DeliveryInstructions", typeof(string)),
            });

            foreach (var order in orders)
            {
                dtCustomers.Rows.Add(order.Created, order.Confirmed, order.DeliveryDate, order.DeliveryTime, order.PickupDate, order.PickupTime, order.ContactName, order.ContactEmail, order.ContactPhone, order.ConfirmationCode,
                                     order.full_address, order.suburb_locality, order.town_city, order.Name, order.Quantity, order.Price, order.ProductSum, order.DeliveryInstructions);
            }

            using (var ms = new MemoryStream())
            {
                using (var document = new XLWorkbook())
                {
                    var worksheet = document.Worksheets.Add(dtCustomers, "Orders");

                    worksheet.Table(0).Theme = XLTableTheme.TableStyleMedium2;
                    worksheet.Table(0).ShowTotalsRow = true;
                    worksheet.Table(0).Field("Quantity").TotalsRowFunction = XLTotalsRowFunction.Sum;
                    worksheet.Table(0).Field("Price").TotalsRowFunction = XLTotalsRowFunction.Sum;
                    worksheet.Table(0).Field("ProductSum").TotalsRowFunction = XLTotalsRowFunction.Sum;
                    worksheet.Column(15).Style.NumberFormat.Format = "#,##0"; //Quantity = 15
                    worksheet.Column(16).Style.NumberFormat.Format = "#,##0.00"; //Price = 16
                    worksheet.Column(17).Style.NumberFormat.Format = "#,##0.00"; //Product Sum = 17
                    worksheet.Columns().AdjustToContents();
                    worksheet.LastRowUsed();
                    document.SaveAs(ms);
                }

                ms.Position = 0;
                return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{excelName}_{DateTime.Now.ToString("yyyy-MM-dd hhmmss")}.xlsx");
            }
        }

        public async Task<ActionResult> OnPostQuickPayAsync(string orderID)
        {
            if (String.IsNullOrEmpty(orderID) || !Guid.TryParse(orderID, out Guid parsedID))
            {
                return new RedirectResult("/Orders/OrdersList");
            }

            if (!await _context.ProductOrder.AnyAsync(p => p.OrderID == parsedID))
            {
                return new RedirectResult("/Orders/OrdersList");
            }

            var order = await _context.ProductOrder.FirstOrDefaultAsync(f => f.OrderID == parsedID);
            if (order == null)
                return new RedirectResult("/Orders/OrdersList");

            var invoice = await _context.Invoice.FirstOrDefaultAsync(f => f.ProductOrderID == parsedID);
            if(invoice == null)
                return new RedirectResult("/Orders/OrdersList");
            var balance = await _context.InvoiceTransaction
                                        .Where(w => w.InvoiceID == invoice.InvoiceID)
                                        .SumAsync(s => s.Quantity * s.Price);

            if (balance == 0) //Check existing balance amount without taking into effect the new transaction amount
            {
                ModelState.AddModelError("BalanceZero", "Cannot add transaction. The order balance is already zero.");
                return Page();
            }

            var InvoiceTransaction = new InvoiceTransaction()
            {
                InvoiceID = invoice.InvoiceID,
                Type = InvoiceHelper.Transaction_Type_Credit,
                Category = InvoiceHelper.Transaction_Category_Payment,
                Price = -1 * balance,
                Quantity = 1,
                Name = "Payment",
                Description = "Payment in full"
            };

            balance += InvoiceTransaction.Price;

            if (balance < 0)
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
            return new RedirectToPageResult("/Orders/OrdersList");
        }
    }
}