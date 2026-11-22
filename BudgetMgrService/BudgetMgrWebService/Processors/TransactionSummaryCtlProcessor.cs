using BudgetMgrServiceLib.Generators;
using BudgetMgrServiceLib.Models;
using BudgetMgrServiceLib.Processors;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BudgetMgrWebService.Processors
{
    public static class TransactionSummaryCtlProcessor
    {
        // TODO: expose the following group types via the web service API, so that the front end
        // can obtain the relevant transactions based upon those types from the back end.

        private static ActionResult<TransactionSummary> CreateTransactionSummary(
            TransactionContent content,
            IEnumerable<Transaction> transactions,
            string groupType)
        {
            if ((groupType != TransactionGroupTypeProcessor.IncomeTransGroupType) &&
                (groupType != TransactionGroupTypeProcessor.ExpenseTransGroupType) &&
                (groupType != TransactionGroupTypeProcessor.MiscTransGroupType))
            {
                return new BadRequestResult();
            }

            var materializedTransactions = transactions.ToArray();
            if (materializedTransactions.Length == 0)
            {
                return new NotFoundResult();
            }

            IEnumerable<string> expenditureGroupNames = groupType switch
            {
                var type when type == TransactionGroupTypeProcessor.ExpenseTransGroupType => materializedTransactions.ToKnownTransNamesOfInterest(content),
                _ => materializedTransactions.ToMiscTransNamesOfInterest(content),
            };

            return materializedTransactions.GenTransactionSummaryData(
                content.IncomeGroupNames,
                expenditureGroupNames);
        }

        public static Task<ActionResult<TransactionSummary>> FindTransactionSummaryAsync(this DataStore _dataStore)
        {
            var content = _dataStore.CreateTransactionContnent();
            return Task.FromResult(CreateTransactionSummary(content, content.Transactions, TransactionGroupTypeProcessor.ExpenseTransGroupType));
        }

        public static Task<ActionResult<TransactionSummary>> FindTransactionSummaryAsync(this DataStore _dataStore, string startDate, string endDate)
        {
            var content = _dataStore.CreateTransactionContnent();
            var transactions = content.Transactions.FilterByDateRange(startDate, endDate);
            return Task.FromResult(CreateTransactionSummary(content, transactions, TransactionGroupTypeProcessor.ExpenseTransGroupType));
        }

        public static Task<ActionResult<TransactionSummary>> FindTransactionSummaryAsync(this DataStore _dataStore, string groupType, string startDate, string endDate)
        {
            var content = _dataStore.CreateTransactionContnent();
            var transactions = content.Transactions.FilterByDateRange(startDate, endDate);
            return Task.FromResult(CreateTransactionSummary(content, transactions, groupType));
        }
    }
}
