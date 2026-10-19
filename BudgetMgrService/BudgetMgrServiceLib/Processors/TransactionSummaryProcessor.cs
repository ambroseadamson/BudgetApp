using System.Collections.Generic;
using BudgetMgrServiceLib.Models;

namespace BudgetMgrServiceLib.Processors
{
    public static class TransactionSummaryProcessor
    {
        public static TransactionSummary ToTransactionSummary(this IEnumerable<Transaction> transactionSrcData,
                                                                   IEnumerable<string> incomeGroupTypes,
                                                                   IEnumerable<string> expenditureGroupTypes)
        {
            var incomeDetails = transactionSrcData.ToCreditDetails(incomeGroupTypes);
            var expenditureDetails = transactionSrcData.ToCreditDetails(expenditureGroupTypes);

            var balance = incomeDetails.ToCreditBalance(expenditureDetails);

            var summaryPct = incomeDetails.ToExpenseSummaryPct(expenditureDetails);

            return TransactionSummary.Of(incomeDetails, expenditureDetails, balance, summaryPct);
        }
    }
}