using System;
using System.Collections.Generic;
using System.Linq;
using BudgetMgrServiceLib.Models;

namespace BudgetMgrServiceLib.Processors
{
    public static class TransactionProcessor
    {
        public static int DayOfMonth(this Transaction transaction) => transaction.DateStamp.Day;
        public static DateTime TransactionDate(this Transaction transaction) => transaction.DateStamp;

        public static string ToDateURLParam(this DateTime dateStamp) => $"{dateStamp.Day}-{dateStamp.Month}-{dateStamp.Year}";

        public static DateTime FromDateURLParam(this string urlParam) => DateTime.Parse(urlParam);

        public static string ToBoolURLParam(this bool param) => (param == true) ? "true" : "false";

        public static bool FromBoolURLParam(this string urlParam) => (urlParam.ToLower() == "true");

        public static string ToDateFilterCfgURLParam(this DateFilterCfg dateFilter) => $"{dateFilter.StartDate.ToDateURLParam()}_{dateFilter.EndDate.ToDateURLParam()}_{dateFilter.UseDateFilter.ToBoolURLParam()}";

        // NOTE: Assumes the collection is sorted by date....
        public static string GenDateFilterCfgURLParam(this IEnumerable<Transaction> transactions, bool useDateFilter)
        {
            return (DateFilterCfg.Of(transactions.First().DateStamp, transactions.Last().DateStamp, useDateFilter)).ToDateFilterCfgURLParam();
        }
        public static DateFilterCfg FromDateFilterCfgURLParam(this string dateFilterURLParam)
        {
            var elements = dateFilterURLParam.Split(new char[] { '_' }).ToArray();
            return DateFilterCfg.Of(elements[0].FromDateURLParam(), elements[1].FromDateURLParam(), elements[2].FromBoolURLParam());
        }
        public static Transaction ApplyUniqueId(Transaction transaction, int _Id) => Transaction.Of(_Id, transaction.DateStamp, transaction.TransType, transaction.Description, transaction.Amount);

    }
}