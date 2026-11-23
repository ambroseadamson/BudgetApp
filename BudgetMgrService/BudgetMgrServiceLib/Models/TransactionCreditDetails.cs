using System.Collections.Generic;

namespace BudgetMgrServiceLib.Models
{
    public record TransactionCreditDetails(IEnumerable<Transaction> Transactions, double Total);
}