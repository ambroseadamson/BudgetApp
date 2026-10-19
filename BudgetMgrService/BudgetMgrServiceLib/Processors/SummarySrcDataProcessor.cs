using BudgetMgrServiceLib.Models;

namespace BudgetMgrServiceLib.Processors
{
    public static class SummarySrcProcessor
    {
        public static SummarySrcData ToSummarySrcData(this TransactionSummary summaryData, string expenditureGroupType)
        {
            return SummarySrcData.Of(expenditureGroupType, summaryData.Expenditure.Total, summaryData.Balance, summaryData.ExpSummaryPct);
        }

    }
}