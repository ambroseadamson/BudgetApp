using System.Collections.Generic;

namespace BudgetMgrServiceLib.Models
{
    public record TransactionCreditDetails(int Id, IEnumerable<Transaction> Transactions, double Total);
}