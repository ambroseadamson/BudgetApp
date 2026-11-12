using BudgetMgrServiceLib.Models;
using Microsoft.AspNetCore.Mvc;

namespace BudgetMgrWebService.Processors
{
    public static class SummaryResultCtlProcessor
    {
        // TODO: expose the following group types via the web service API, so that the front end
        // can obtain the relevant transactions based upon those types from the back end.

        private static readonly string expenseGroupType = "expense";
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
                : SummaryResult.Of(content, materializedTransactions, groupType);
        }

        public static Task<ActionResult<SummaryResult>> ToSummaryResultAsync(this TransactionContext _context, string? groupType = null)
        {
            var content = TransactionContent.Of(_context);
            return Task.FromResult(CreateSummaryResult(content, content.Transactions, groupType ?? expenseGroupType)); ;
        }

        public static Task<ActionResult<SummaryResult>> FindSummaryResultAsync(this TransactionContext _context, string startDate, string endDate)
        {
            var content = TransactionContent.Of(_context);
            var transactions = content.Transactions.FilterByDateRange(startDate, endDate);
            return Task.FromResult(CreateSummaryResult(content, transactions, expenseGroupType));
        }

        public static Task<ActionResult<SummaryResult>> FindSummaryResultAsync(this TransactionContext _context, string startDate, string endDate, string groupType)
        {
            var content = TransactionContent.Of(_context);
            var transactions = content.Transactions.FilterByDateRange(startDate, endDate);

            return Task.FromResult(CreateSummaryResult(content, transactions, groupType));
        }

    }
}
