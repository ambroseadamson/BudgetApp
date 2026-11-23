using BudgetMgrServiceLib.Models;

public record SummaryResult(TransactionCreditDetails Income, DateFilter DateRange, IEnumerable<TransactionSummaryEntry> SummaryData);