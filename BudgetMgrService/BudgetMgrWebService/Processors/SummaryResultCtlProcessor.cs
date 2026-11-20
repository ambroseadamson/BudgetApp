using BudgetMgrServiceLib.Models;
using BudgetMgrServiceLib.Processors;
using Microsoft.AspNetCore.Mvc;

namespace BudgetMgrWebService.Processors
{
    public static class SummaryResultCtlProcessor
    {
        // TODO: expose the following group types via the web service API, so that the front end
        // can obtain the relevant transactions based upon those types from the back end.

        public static IEnumerable<Transaction> ExcludeIncomeTransactions(
            this IEnumerable<Transaction> transactions,
            IEnumerable<string> incomeGroupNames)
        {
            return transactions.Where(transaction =>
                incomeGroupNames.Contains(transaction.Description) == false);
        }

        private static ActionResult<SummaryResult> CreateSummaryResult(TransactionContent content,
                                                                       IEnumerable<Transaction> transactions,
                                                                       string groupType)
        {
            var materializedTransactions = transactions.ToArray();

            return materializedTransactions.Length == 0
                ? new NotFoundResult()
                : content.CreateSummaryResult(materializedTransactions, groupType);
        }

        public static Task<ActionResult<SummaryResult>> ToSummaryResultAsync(this DataStore _dataStore, string? groupType = null)
        {
            var content = _dataStore.CreateTransactionContnent();
            return Task.FromResult(CreateSummaryResult(content, content.Transactions, groupType ?? TransactionGroupTypeProcessor.ExpenseTransGroupType)); ;
        }

        public static Task<ActionResult<SummaryResult>> FindSummaryResultAsync(this DataStore _dataStore, string startDate, string endDate)
        {
            var content = _dataStore.CreateTransactionContnent();
            var transactions = content.Transactions.FilterByDateRange(startDate, endDate);
            return Task.FromResult(CreateSummaryResult(content, transactions, TransactionGroupTypeProcessor.ExpenseTransGroupType));
        }

        public static Task<ActionResult<SummaryResult>> FindSummaryResultAsync(this DataStore _dataStore, string startDate, string endDate, string groupType)
        {
            var content = _dataStore.CreateTransactionContnent();
            var transactions = content.Transactions.FilterByDateRange(startDate, endDate);

            return Task.FromResult(CreateSummaryResult(content, transactions, groupType));
        }

    }
}
