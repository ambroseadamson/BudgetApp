namespace BudgetMgrServiceLib.Models
{
    public record TransactionSummary(TransactionCreditDetails Income, TransactionCreditDetails Expenditure, double Balance, double ExpSummaryPct);

    public record TransactionSummaryEntry(string Name, double Pct);

}