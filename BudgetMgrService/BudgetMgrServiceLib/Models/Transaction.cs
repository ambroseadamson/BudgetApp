using System;
using System.Collections.Generic;

namespace BudgetMgrServiceLib.Models
{
    public record Transaction(long Id, DateTime DateStamp, string TransType, string Description, double Amount);
    public record TransactionCreditDetails(IEnumerable<Transaction> Transactions, double Total);
    public record TransactionResults(string ResultName, IEnumerable<Transaction> Results, string DateRangeURLParam);
    public record TransactionSummary(TransactionCreditDetails Income, TransactionCreditDetails Expenditure, double Balance, double ExpSummaryPct);
    public record TransactionSummaryEntry(string Name, double Pct);
}