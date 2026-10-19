using System;
using System.Collections.Generic;
using System.Linq;
namespace BudgetMgrServiceLib.Models
{
    public class TransactionSummary
    {
        public TransactionCreditDetails Income { get; set; }
        public TransactionCreditDetails Expenditure { get; set; }

        public double Balance { get; set; }

        public double ExpSummaryPct { get; set; }
        public static TransactionSummary Of(TransactionCreditDetails income,
                                            TransactionCreditDetails expenditure,
                                            double balance,
                                            double expSummaryPct)
        {
            return new TransactionSummary()
            {
                Income = income,
                Expenditure = expenditure,
                Balance = balance,
                ExpSummaryPct = expSummaryPct
            };
        }


    }

    public class TransactionSummaryEntry
    {
        public string Name { set; get; }
        public double Pct { get; set; }

        public static TransactionSummaryEntry Of(string name, TransactionSummary summary)
        {
            return new TransactionSummaryEntry()
            {
                Name = name,
                Pct = summary.ExpSummaryPct,
            };
        }
    }

}