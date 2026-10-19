using System;
using System.Collections.Generic;
using System.Linq;
using BudgetMgrServiceLib.Models;

namespace BudgetMgrServiceLib.Models
{
    public class CashReceipt
    {
        public DateTime DateStamp { get; set; }
        public string Desc { get; set; }
        public double Total { get; set; }
        public double CashPaid { get; set; }

        public static CashReceipt Of(DateTime dateStamp, string desc, double total, double cashPaid)
        {
            return new CashReceipt()
            {
                DateStamp = dateStamp,
                Desc = desc,
                Total = total,
                CashPaid = cashPaid
            };
        }
    }

}