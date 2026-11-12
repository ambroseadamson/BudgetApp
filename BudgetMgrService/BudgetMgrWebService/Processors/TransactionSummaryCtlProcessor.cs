using BudgetMgrServiceLib.Models;
using BudgetMgrServiceLib.Generators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BudgetMgrWebService.Processors
{
    public static class TransactionSummaryCtlProcessor
    {
        // TODO: expose the following group types via the web service API, so that the front end
        // can obtain the relevant transactions based upon those types from the back end.

        private static readonly string expenseGroupType = "expense";

        private static ActionResult<TransactionSummary> CreateTransactionSummary(
            TransactionContent content,
            IEnumerable<Transaction> transactions,
            string groupType)
        {
            var materializedTransactions = transactions.ToArray();
            if (materializedTransactions.Length == 0)
            {
                return new NotFoundResult();
            }

            IEnumerable<string> expenditureGroupNames = groupType switch
            {
                var type when type == expenseGroupType => materializedTransactions.ToKnownTransNamesOfInterest(content),
                _ => materializedTransactions.ToMiscTransNamesOfInterest(content),
            };

            return materializedTransactions.GenTransactionSummaryData(
                content.IncomeGroupNames,
                expenditureGroupNames);
        }

        public static Task<ActionResult<TransactionSummary>> FindTransactionSummaryAsync(this TransactionContext _context)
        {
            var content = TransactionContent.Of(_context);
            return Task.FromResult(CreateTransactionSummary(content, content.Transactions, expenseGroupType));
        }

        public static Task<ActionResult<TransactionSummary>> FindTransactionSummaryAsync(this TransactionContext _context, string startDate, string endDate)
        {
            var content = TransactionContent.Of(_context);
            var transactions = content.Transactions.FilterByDateRange(startDate, endDate);
            return Task.FromResult(CreateTransactionSummary(content, transactions, expenseGroupType));
        }

        public static Task<ActionResult<TransactionSummary>> FindTransactionSummaryAsync(this TransactionContext _context, string groupType, string startDate, string endDate)
        {
            var content = TransactionContent.Of(_context);
            var transactions = content.Transactions.FilterByDateRange(startDate, endDate);
            return Task.FromResult(CreateTransactionSummary(content, transactions, groupType));
        }
    }
}
