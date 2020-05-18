using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using littlebreadloaf.Data;
using System.IO;
using ClosedXML.Excel;

namespace littlebreadloaf.Pages.Orders
{
    public class StatementTransaction
    {
        public DateTime TransactionDate { get; set; }
        public DateTime ProcessedDate { get; set; }
        public string Type { get; set; }
        public string Details { get; set; }
        public string Particulars { get; set; }
        public string Code { get; set; }
        public string Reference { get; set; }
        public decimal Amount { get; set; }
        public string OrderID { get; set; }
    }

    public enum StatementColumns : int
    {
        TransactionDate = 1,
        ProcessedDate = 2,
        Type = 3,
        Details = 4,
        Particulars = 5,
        Code = 6,
        Reference = 7,
        Amount = 8
    }

    [Authorize]
    public class OrderStatementUploadModel : PageModel
    {
        private readonly ProductContext _context;
        public OrderStatementUploadModel(ProductContext context)
        {
            _context = context;
        }

        [BindProperty]
        public List<StatementTransaction> StatementTransactions { get; set; }


        [BindProperty]
        public IFormFile FileUpload { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostUploadAsync()
        {

            if(FileUpload == null)
            {
                ModelState.AddModelError("File.Missing", "Choose a file to upload.");
                return Page();
            }

            using (var ms = new MemoryStream())
            {
                FileUpload.CopyTo(ms);
                using (var document = new XLWorkbook(ms))
                {
                    var rows = document.Worksheet(1).Rows().Skip(1); //Ignore header record

                    foreach (var row in rows)
                    {
                        decimal amount = 0;
                        DateTime transactionDate = new DateTime(9999, 12, 31);
                        DateTime processedDate = new DateTime(9999, 12, 31);

                        if(!String.IsNullOrEmpty(row.Cell((int)StatementColumns.Amount).Value.ToString()))
                            amount = Decimal.Parse(row.Cell((int)StatementColumns.Amount).Value.ToString().Replace("$", ""));

                        if (!string.IsNullOrEmpty(row.Cell((int)StatementColumns.TransactionDate).Value.ToString()))
                            transactionDate = DateTime.Parse(row.Cell((int)StatementColumns.TransactionDate).Value.ToString());

                        if(!string.IsNullOrEmpty(row.Cell((int)StatementColumns.ProcessedDate).Value.ToString()))
                            processedDate = DateTime.Parse(row.Cell((int)StatementColumns.ProcessedDate).Value.ToString());

                        StatementTransactions.Add(new StatementTransaction()
                        {
                            TransactionDate = transactionDate,
                            ProcessedDate = processedDate,
                            Details = row.Cell((int)StatementColumns.Details).Value.ToString(),
                            Particulars = row.Cell((int)StatementColumns.Particulars).Value.ToString().ToUpper(),
                            Code = row.Cell((int)StatementColumns.Code).Value.ToString().ToUpper(),
                            Reference = row.Cell((int)StatementColumns.Reference).Value.ToString().ToUpper(),
                            Amount = amount
                        }); 

                    }
                }
            }

            var orders = await OrdersHelper.GetActiveOrders(_context);

            foreach(var trans in StatementTransactions)
            {
                var confirmation = "";
                if (trans.Particulars.Length == 6)
                {
                    confirmation = trans.Particulars;
                }
                else if (trans.Code.Length == 6)
                {
                    confirmation = trans.Code;
                }
                else if(trans.Reference.Length == 6)
                {
                    confirmation = trans.Reference;
                }
                var order = orders.FirstOrDefault(f => f.ConfirmationCode == confirmation);
                if(order != null)
                {
                    trans.OrderID = order.OrderID.Value.ToString();
                }

            }

            return Page();
        }

        public async Task<IActionResult> OnPostProcessAsync()
        {

            foreach(var trans in StatementTransactions)
            {
                if (String.IsNullOrEmpty(trans.OrderID) || !Guid.TryParse(trans.OrderID, out Guid parsedID))
                    continue;
                var order = await _context.ProductOrder.FirstOrDefaultAsync(f => f.OrderID == parsedID);
                if (order == null)
                    continue;
                var invoice = await _context.Invoice.FirstOrDefaultAsync(f => f.ProductOrderID == parsedID);
                if (invoice == null)
                    continue;
                var balance = await _context.InvoiceTransaction
                                        .Where(w => w.InvoiceID == invoice.InvoiceID)
                                        .SumAsync(s => s.Quantity * s.Price);

                if (balance == 0) //Check existing balance amount without taking into effect the new transaction amount
                    continue;

                var InvoiceTransaction = new InvoiceTransaction()
                {
                    InvoiceID = invoice.InvoiceID,
                    Type = InvoiceHelper.Transaction_Type_Credit,
                    Category = InvoiceHelper.Transaction_Category_Payment,
                    Price = -1 * balance,
                    Quantity = 1,
                    Name = "Payment",
                    Description = $"Bank transaction - {trans.TransactionDate.ToString("yyyy-MM-dd hh:mm:ss")}"
                };

                balance += InvoiceTransaction.Price;

                if (balance < 0)
                    continue;

                if (balance == 0)
                {
                    order.Payment = DateTime.Now;
                    _context.ProductOrder.Update(order);
                }
                await _context.InvoiceTransaction.AddAsync(InvoiceTransaction);
            }

            await _context.SaveChangesAsync();
            return new RedirectToPageResult("/Orders/OrdersList");
        }
    }
}
