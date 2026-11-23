using BudgetMgrServiceLib.Models;

public record SummaryResult(int Id, TransactionCreditDetails Income, DateFilter DateRange, IEnumerable<TransactionSummaryEntry> SummaryData);