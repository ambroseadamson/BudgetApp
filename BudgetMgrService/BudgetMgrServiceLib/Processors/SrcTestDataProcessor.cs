using System;
using System.Collections.Generic;
using System.Linq;
using BudgetMgrServiceLib.Models;
using BudgetMgrServiceLib.Generators;

namespace BudgetMgrServiceLib.Processors
{
    public static class SrcTestDataProcessor
    {
        public static T ToTransaction<T>(this IEnumerable<Func<T, T>> fns, T init) => fns.Aggregate(init, (currVal, fn) => fn(currVal));

        public static IEnumerable<T> ToOrderedTransactions<T>(this IEnumerable<T> transactions, Func<T, int> dayOfMonth) => transactions.OrderBy(transaction => dayOfMonth(transaction));

        public static IEnumerable<int> TransactionRange(this TestDataCfg testDataCfg) => Enumerable.Range(0, testDataCfg.NumberOfTransactions);

        public static string AsDateStampString<T>(this T transaction, string dateFormat, Func<T, DateTime> date) => date(transaction).ToString(dateFormat);

        public static string GenSrcFileName<T>(this IEnumerable<T> transData, string dateFmt, string fileNamePrefix, Func<T, DateTime> date)
        {
            return string.Format("{0}{1}-to-{2}.csv",
                                fileNamePrefix,
                                AsDateStampString(transData.First(), dateFmt, date),
                                AsDateStampString(transData.Last(), dateFmt, date));
        }

        public static T RandomElement<T>(this Random rnd, IEnumerable<T> collection) => collection.ElementAt(rnd.ToRandomSrcVal(0, collection.Count() - 1));


        public static IEnumerable<string> ObtainCsvData(Func<TestDataCfg, GroupTypesCfgEx, DateTime, Random, SrcData> genSrcData, 
                                                        TestDataCfg testDataCfg, 
                                                        GroupTypesCfgEx groupTypesCfg) 
        {
            return genSrcData(testDataCfg, groupTypesCfg, DateTime.Now, new Random()).Data;
        }

        public static IEnumerable<Transaction> ObtainTransactionData(Func<TestDataCfg, GroupTypesCfgEx, DateTime, Random, SrcData> genSrcData, TestDataCfg testDataCfg, GroupTypesCfgEx groupTypesCfg)
        {
            Func<IEnumerable<Transaction>, IEnumerable<int>> fnUniqueIds = (transactions) => transactions.GenerateUniqueIds(transactions, (trans) => (int)trans.Id);
            Func<IEnumerable<Transaction>, IEnumerable<Transaction>> applyUniqueIds = (transactions) => transactions.Zip(fnUniqueIds(transactions), TransactionProcessor.ApplyUniqueId);
            var srcData = genSrcData(testDataCfg, groupTypesCfg, DateTime.Now, new Random());
            return TransactionGenerator.GenTransactionData(srcData.Data, testDataCfg, groupTypesCfg, srcData.Type, applyUniqueIds).Results;
        }

        public static bool NumericValueIsValid(this string numericValue)
        {
            bool parseSucceeded = true;
            try
            {
                Double.Parse(numericValue);
            }
            catch (Exception)
            {
                parseSucceeded = false;
            }
            return parseSucceeded;
        }

        public static IEnumerable<T> AppendMiscTransactions<T>(this IEnumerable<T> transactions,
                                                               TestDataCfg testDataCfg,
                                                               Func<T, TestDataCfg, T> fnGenSrcMiscTransaction,
                                                               Func<T, string, double, T> fnGenMiscTransaction)
        {
            var noOfSrcTransactions = transactions.Count() - testDataCfg.MiscTransactionNames.Count();
            var srcTransactions = transactions.Take(noOfSrcTransactions);
            var srcForMiscTransactions = transactions.Skip(noOfSrcTransactions)
                                                     .Select(transaction => fnGenSrcMiscTransaction(transaction, testDataCfg));

            var miscTransactions = srcForMiscTransactions.Zip(testDataCfg.MiscTransactionNames,
                                                              (transaction, miscTransactionName) => fnGenMiscTransaction(transaction, miscTransactionName, testDataCfg.MiscTransactionValue));

            return srcTransactions.Concat(srcForMiscTransactions)
                                  .Concat(miscTransactions);
        }
    }
}
