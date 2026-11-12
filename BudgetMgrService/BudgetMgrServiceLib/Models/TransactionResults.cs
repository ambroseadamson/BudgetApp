using System.Collections.Generic;

namespace BudgetMgrServiceLib.Models
{
    public record TransactionResults(string ResultName, IEnumerable<Transaction> Results, string DateRangeURLParam);
}