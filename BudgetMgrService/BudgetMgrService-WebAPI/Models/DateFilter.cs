using System;
using System.Collections.Generic;
using System.Linq;
using BudgetMgrServiceLib.Models;

public class DateFilter
{
    public DateTime StartDate { set; get; }
    public DateTime EndDate { get; set; }

    public static DateFilter Of(IEnumerable<Transaction> transactions)
    {
        var sortedTransactions = transactions.OrderBy(transaction => transaction.DateStamp);
        return new DateFilter()
        {
            StartDate = sortedTransactions.First().DateStamp,
            EndDate = sortedTransactions.Last().DateStamp
        };
    }
}
