using System.Collections.Generic;
using System.Linq;
using Xunit;
using BudgetMgrServiceLib.Models;
using BudgetMgrServiceLib.Processors;
using System;
using BudgetMgrServiceLib.Generators;

namespace BudgetMgrServiceLibTest
{
    public class ProperyLensTests
    {
        public static readonly TestDataCfg testDataCfg = AppCfgProcessor.ToTestDataCfg();
        public static readonly GroupTypesCfgEx groupTypesCfg = AppCfgProcessor.ToGroupTypesCfgEx();
        public static readonly FunctionalComparer<TransactionGroupCfg> transGroupTypeComparer = new FunctionalComparer<TransactionGroupCfg>()
        {
            comparer = (x, y) => ((x.Id == y.Id) && (x.GroupName == y.GroupName) && (x.Names == y.Names)),
            hashCodeGenerator = (x) => (x.Id.GetHashCode() + x.GroupName.GetHashCode() + x.Names.GetHashCode())
        };

        private static Func<string, bool> isMiscValue = (miscValue) => (miscValue.First() == '*') && (miscValue.Last() == '*');

        public static readonly DateTime centuryBegin = new DateTime(2001, 1, 1);
        public static readonly DateTime currentDate = DateTime.Now;
        public static readonly DateTime startDate = new DateTime(currentDate.Year, currentDate.Month, 2);
        public static readonly DateTime endDate = new DateTime(currentDate.Year, currentDate.Month, 28);

        public static Func<TestDataCfg, GroupTypesCfgEx, DateTime, Random, SrcData> genSrcData = StatementGenerator.GenStatementData;

        public static IEnumerable<Transaction> ToMiscTransactions(IEnumerable<Transaction> transactions)
        {
            Func<Transaction, string> genTransType = (currTrans) => $"*{currTrans.TransType}*";
            Func<Transaction, string> genDebitName = (currTrans) => $"*{currTrans.Description}*";

            var applyDateStamp = PropertyLens<Transaction, DateTime>.Of(currTrans => currTrans.DateStamp, (currTrans, dateStamp) => currTrans.DateStamp = dateStamp);
            var applyTransType = PropertyLens<Transaction, string>.Of(currTrans => currTrans.TransType, (currTrans, transType) => currTrans.TransType = transType);
            var applyDebitName = PropertyLens<Transaction, string>.Of(currTrans => currTrans.Description, (currTrans, debitName) => currTrans.Description = debitName);
            var applyAmount = PropertyLens<Transaction, double>.Of(currTrans => currTrans.Amount, (currTrans, amount) => currTrans.Amount = amount);

            var srcValueFns = new List<Func<Transaction, Transaction>> {
                (currTrans) => currTrans.Map(applyTransType,genTransType(currTrans)),     // TransType
                (currTrans) => currTrans.Map(applyDebitName,genDebitName(currTrans))     // DebitName
            };

            Func<Transaction, bool> isTransactionOfInterest = (currTrans) =>
            {
                return groupTypesCfg.ToDebitTypesOfInterestEx().Contains(currTrans.TransType) &&
                       groupTypesCfg.ToDebitNamesOfInterestWithDefault().Contains(currTrans.Description);
            };

            return transactions.Where(currTrans => isTransactionOfInterest(currTrans)) // Filter out credit transactions
                               .Select(currTrans => srcValueFns.ToTransaction(currTrans))                            // convert to misc transactions (misc type and msic description)
                               .ToArray();
        }

        public static IEnumerable<object[]> ObtainTransactions()
        {
            var transactions = ToMiscTransactions(SrcTestDataProcessor.ObtainTransactionData(genSrcData, testDataCfg, groupTypesCfg));


            for (int i = 0; i < transactions.Count(); i++)
            {
                yield return new object[] { transactions.ElementAt(i) };
            }
        }

        // Test where it validates that state changes after the lens has been applied to the transaction.
        [Theory]
        [MemberData(nameof(ObtainTransactions))]
        public void Test1_ValidateLens(Transaction currTrans)
        {
            Func<Transaction, string> deMiscifyTransName = (_currTrans) => _currTrans.Description.Replace('*', ' ').Trim();

            var applyDebitName = PropertyLens<Transaction, string>.Of(_currTrans => _currTrans.Description, (__currTrans, debitName) => __currTrans.Description = debitName);

            var srcValueFns = new List<Func<Transaction, Transaction>> {
                (_currTrans) => _currTrans.Map(applyDebitName,deMiscifyTransName(currTrans))     // DebitName
            };


            Assert.True(isMiscValue(currTrans.Description));

            var newTrans = srcValueFns.ToTransaction(currTrans);

            Assert.False(isMiscValue(newTrans.Description));
        }
    }
}