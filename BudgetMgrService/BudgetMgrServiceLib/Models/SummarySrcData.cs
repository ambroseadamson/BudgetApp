namespace BudgetMgrServiceLib.Models
{
    public class SummarySrcData
    {
        public string ExpenditureGroupType { set; get; }
        public double Total { set; get; }
        public double Balance { set; get; }
        public double SummaryPct { set; get; }

        public static SummarySrcData Of(string expenditureGroupType, double total, double balance, double summaryPct)
        {
            return new SummarySrcData()
            {
                ExpenditureGroupType = expenditureGroupType,
                Total = total,
                Balance = balance,
                SummaryPct = summaryPct
            };
        }
    }
}