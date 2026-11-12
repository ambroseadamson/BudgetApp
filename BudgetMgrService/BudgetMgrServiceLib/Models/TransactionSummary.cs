namespace BudgetMgrServiceLib.Models
{
    public record TransactionSummary(TransactionCreditDetails Income, TransactionCreditDetails Expenditure, double Balance, double ExpSummaryPct);

    public record TransactionSummaryEntry(int Id, string Name, double Pct);

}