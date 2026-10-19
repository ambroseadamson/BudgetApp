using System.Collections.Generic;
using System.Linq;
using BudgetMgrServiceLib.Models;
namespace BudgetMgrServiceLib.Processors
{
    public static class TransactionCreditDetailsProcessor
    {
        public static TransactionCreditDetails ToCreditDetails(this IEnumerable<Transaction> transactions,
                                                                          IEnumerable<string> groupTypes)
        {
            var _transactions = transactions.Where(currTrans => currTrans.IsTransactionOfInterest(groupTypes));
            return TransactionCreditDetails.Of(_transactions, _transactions.Sum(currTrans => currTrans.Amount));
        }

        public static double ToCreditBalance(this TransactionCreditDetails income, TransactionCreditDetails expense) => income.Total - expense.Total;

        public static double ToExpenseSummaryPct(this TransactionCreditDetails income, TransactionCreditDetails expense) => ((income.Total == 0) || (expense.Total == 0)) ? 0 : (expense.Total / income.Total) * 100;


    }
}