using System;
using System.Collections.Generic;
using System.Linq;
using BudgetMgrServiceLib.Models;

namespace BudgetMgrServiceLib.Processors
{
    public static class TransactionFilteringProcessor
    {
        public static readonly DateTime earliestDate = new DateTime(2001, 1, 1);
        public static readonly DateTime todaysDate = DateTime.Now;

        public static TimeSpan ToTimeSpan(this DateTime date) => new TimeSpan(date.Ticks - earliestDate.Ticks);

        public static bool FilterByDate(this Transaction transaction, DateFilterCfg dateFilter)
        {
            return (transaction.DateStamp.ToTimeSpan() >= dateFilter.StartDate.ToTimeSpan()) &&
                   (transaction.DateStamp.ToTimeSpan() <= dateFilter.EndDate.ToTimeSpan());
        }

        public static bool NoFiltering(this Transaction transaction, DateFilterCfg dateFilter) => true;

        public static bool IsTransactionOfInterest(this Transaction transaction, IEnumerable<string> namesOfInterest)
        {
            return namesOfInterest.Any(name => transaction.Description.ToUpper() == name.ToUpper());
        }

        public static IEnumerable<string> TransactionNamesOfInterest(this GroupTypesCfgEx groupTypesCfg, string groupTypeName)
        {
            return groupTypesCfg.ToTransNamesEx().First(group => group.GroupName == groupTypeName).Names;
        }

        public static bool TransactionOfInterest(this Transaction transaction, GroupTypesCfgEx groupTypesCfg, string groupTypeName)
        {
            return transaction.IsTransactionOfInterest(groupTypesCfg.TransactionNamesOfInterest(groupTypeName));
        }

        public static IEnumerable<Transaction> ByDateRange(this IEnumerable<Transaction> transactions, GroupTypesCfgEx groupTypesCfg, DateFilterCfg dateFilter)
        {
            return transactions.Where(transaction => (dateFilter.UseDateFilter == true) ? transaction.FilterByDate(dateFilter) : transaction.NoFiltering(dateFilter));
        }

        public static IEnumerable<Transaction> ByTransGroupType(this IEnumerable<Transaction> transactions, GroupTypesCfgEx groupTypesCfg, string transGroupType, bool invertGroupTypeFilter)
        {
            return transactions.Where(transaction =>
            {
                return (invertGroupTypeFilter == true)
                    ? (transaction.TransactionOfInterest(groupTypesCfg, transGroupType) == false)
                    : transaction.TransactionOfInterest(groupTypesCfg, transGroupType);
            });
        }
    }
}