using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace littlebreadloaf
{

    public static class InvoiceHelper
    {
        public const string Transaction_Type_Debit = "Debit";
        public const string Transaction_Type_Credit = "Credit";

        public const string Transaction_Catgory_Product = "Product";
        public const string Transaction_Category_Discount = "Discount";
        public const string Transaction_Category_Payment = "Payment";

        public static DateTime GetDueDate(int numberOfDays)
        {
            return DateTime.Now.AddDays(numberOfDays);
        }
    }
}
