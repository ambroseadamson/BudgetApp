using System.Collections.Generic;
using BudgetMgrServiceLib.Models;
using BudgetMgrServiceLib.Processors;

namespace BudgetMgrServiceLib.Generators
{
    public static class TransactionSummaryGenerator
    {
        public static TransactionSummary GenTransactionSummaryData(this IEnumerable<Transaction> transactionSrcData,
                                                                   IEnumerable<string> incomeGroupNames,
                                                                   IEnumerable<string> expenditureGroupNames)
        {
            var incomeDetails = transactionSrcData.ToCreditDetails(incomeGroupNames);
            var expenditureDetails = transactionSrcData.ToCreditDetails(expenditureGroupNames);

            var balance = incomeDetails.ToCreditBalance(expenditureDetails);

            var summaryPct = incomeDetails.ToExpenseSummaryPct(expenditureDetails);

            return TransactionSummary.Of(incomeDetails, expenditureDetails, balance, summaryPct);
        }
    }
}