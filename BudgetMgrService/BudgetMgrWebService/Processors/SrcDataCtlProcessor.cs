using BudgetMgrServiceLib.Models;
using BudgetMgrServiceLib.Processors;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace BudgetMgrWebService.Processors
{
    public static class SrcDataCtlProcessor
    {
        public static Transaction ApplyUniqueId(Transaction transaction, int _Id) => new Transaction(_Id, transaction.DateStamp, transaction.TransType, transaction.Description, transaction.Amount);

        public static IEnumerable<Transaction> ApplyUniqueIds(this TransactionContext context, IEnumerable<Transaction> transactions)
        {
            var ids = context.Transactions.GenerateUniqueIds(transactions, (trans) => (int)trans.Id).ToArray();
            return transactions.Zip(ids, ApplyUniqueId);
        }

        public static IEnumerable<Transaction> RemoveDuplicates(this IEnumerable<Transaction> _srcData, DbSet<Transaction> ctxTransactions)
        {
            var allTransactions = Option.Some<IEnumerable<Transaction>>(ctxTransactions.ToArray())
                                        .ValueOr(Enumerable.Empty<Transaction>());

            return _srcData.Where(lhs =>
            {
                return allTransactions.Any(rhs =>
                {
                    return ((lhs.DateStamp.CompareTo(rhs.DateStamp) == 0) &&
                            (lhs.TransType == rhs.TransType) &&
                            (lhs.Description == rhs.Description) &&
                            (lhs.Amount == rhs.Amount));
                }) == false;
            });
        }

    }
}
