using System;
using System.Collections.Generic;
using System.Linq;

namespace BudgetMgrServiceLib.Models
{
    public class TransactionCreditDetails
    {
        public IEnumerable<Transaction> Transactions { get; set; }

        public double Total { get; set; }

        public static TransactionCreditDetails Of(IEnumerable<Transaction> transactions, double total)
        {
            return new TransactionCreditDetails()
            {
                Transactions = transactions,
                Total = total
            };
        }
    }
}