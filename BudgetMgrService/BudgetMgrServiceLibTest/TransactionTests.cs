using System;
using System.Collections.Generic;
using System.Linq;
using BudgetMgrServiceLib.Models;
using BudgetMgrServiceLib.Processors;
using BudgetMgrServiceLib.Generators;
using Xunit;

// Extensions methods used in the tests:
// AppCfgProcessor.ToTestDataCfg
// AppCfgProcessor.ToGroupTypesCfgEx
// StatementGenerator.GenCashReceiptData
// SrcTestDataProcessor.ObtainCsvData
// SrcTestDataProcessor.ObtainTransactionData
// TransactionSummaryProcessor.GenTransactionSummaryData
// TransactionSummaryProcessor.ToSummarySrcData
// TransactionSummaryProcessor.ToCreditDetails
// TransactionSummaryProcessor.ToTotalIncomeGroup
// TransactionSummaryProcessor.ToTotalExpenseGroup
// TransactionSummaryProcessor.ToCreditDetails
// TransactionSummaryProcessor.ToCreditBalance
// TransactionSummaryProcessor.ToExpenseSummaryPct
// TransactionGroupTypeProcessor.ToDebitNamesOfInterestWithDefault
// TransactionGroupTypeProcessor.ToDebitTypesOfInterestEx
// TransactionGroupTypeProcessor.ToTransTypesEx
// TransactionGroupTypeProcessor.ToGroupTypesToBeAdded
// SrcTestDataProcessor.NumericValueIsValid
// DateFilterCfg.Of

namespace BudgetMgrServiceLibTest
{
    public class TransactionTests
    {

        public static readonly TestDataCfg testDataCfg = AppCfgProcessor.ToTestDataCfg();
        public static readonly GroupTypesCfgEx groupTypesCfg = AppCfgProcessor.ToGroupTypesCfgEx();

        public static readonly DateTime centuryBegin = new DateTime(2001, 1, 1);
        public static readonly DateTime currentDate = DateTime.Now;
        public static readonly DateTime startDate = new DateTime(currentDate.Year, currentDate.Month, 2);
        public static readonly DateTime endDate = new DateTime(currentDate.Year, currentDate.Month, 28);

        public static Func<TestDataCfg, GroupTypesCfgEx, DateTime, Random, SrcData> genSrcData = StatementGenerator.GenStatementData;

        public static IEnumerable<object[]> ObtainTransactions()
        {
            var transactions = SrcTestDataProcessor.ObtainTransactionData(genSrcData, testDataCfg, groupTypesCfg);
            for (int i = 0; i < transactions.Count(); i++)
            {
                yield return new object[] { transactions.ElementAt(i) };
            }
        }

        public static IEnumerable<object[]> ObtainTransactionGroups()
        {
            var transactionGroups = groupTypesCfg.ToTransNamesEx();
            for (int i = 0; i < transactionGroups.Count(); i++)
            {
                yield return new object[] { transactionGroups.ElementAt(i) };
            }
        }

        public class TransactionDateRange
        {
            public TransactionDateRange(DateTime _startDate, DateTime _endDate, DateTime _currentDate)
            {
                this.StartSpan = new TimeSpan(_startDate.Ticks - centuryBegin.Ticks);
                this.EndSpan = new TimeSpan(_endDate.Ticks - centuryBegin.Ticks);
                this.CurrentSpan = new TimeSpan(_currentDate.Ticks - centuryBegin.Ticks);
            }

            public TimeSpan StartSpan { get; }
            public TimeSpan EndSpan { get; }
            public TimeSpan CurrentSpan { get; }

        }

        // Dates are within expected range from AppCfg (timeStamp within date range from DateFilterCfg)
        [Theory]
        [MemberData(nameof(ObtainTransactions))]
        public void Test1_DatesAreWithinExpectedRange(Transaction transaction)
        {
            var dateRange = new TransactionDateRange(startDate, endDate, transaction.DateStamp);
            Assert.InRange(dateRange.CurrentSpan, dateRange.StartSpan, dateRange.EndSpan);
        }

        // // Transaction types are valid (TransType should exist in range of known types)
        [Theory]
        [MemberData(nameof(ObtainTransactions))]
        public void Test2_TransactionsValuesAreValid(Transaction transaction)
        {
            Assert.Contains(transaction.TransType, groupTypesCfg.ToTransTypesEx());
        }


        // // Description values are valid (Description should exist in range of known descriptions)
        [Theory]
        [MemberData(nameof(ObtainTransactions))]
        public void Test3_DescriptionValuesAreValid(Transaction transaction)
        {
            Assert.Contains(transaction.Description,
                            groupTypesCfg.ToDebitNamesOfInterestWithDefault()
                                         .Concat(groupTypesCfg.ToCreditNamesOfInterestWithDefault()));
        }

        // // Amount values are valid (Amount should be a positive value)
        [Theory]
        [MemberData(nameof(ObtainTransactions))]
        public void Test4_AmountValuesAreValid(Transaction transaction)
        {
            Assert.True(transaction.Amount >= 0);
        }

        // Transactions Are From Selected Group (Description from transaction should exist in range of known description from selected group)
        [Theory]
        [MemberData(nameof(ObtainTransactionGroups))]
        public void Test5_TransactionsAreFromSelectedGroup(TransactionGroupCfg transNameGroup)
        {
            var _transactions = SrcTestDataProcessor.ObtainTransactionData(genSrcData, testDataCfg, groupTypesCfg).ToArray();

            var transactions = _transactions.ByTransGroupType(groupTypesCfg, transNameGroup.GroupName, false);

            Assert.True(transactions.All(transaction => transNameGroup.Names.Contains(transaction.Description)));
        }

        // Transactions Are From Selected Inverted Group (Description from transaction should exist in range of known description from selected group)
        [Theory]
        [MemberData(nameof(ObtainTransactionGroups))]
        public void Test6_TransactionsAreFromSelectedInvertedGroup(TransactionGroupCfg transNameGroup)
        {
            var _transactions = SrcTestDataProcessor.ObtainTransactionData(genSrcData, testDataCfg, groupTypesCfg).ToArray();


            var transactions = _transactions.ByTransGroupType(groupTypesCfg, transNameGroup.GroupName, true);

            foreach (var transaction in transactions)
            {
                Assert.DoesNotContain(transaction.Description, transNameGroup.Names);
            }
        }

        // Transactions Are From Selected Date Range
        [Fact]
        public void Test7_TransactionsAreFromSelectedDateRange()
        {
            var _transactions = SrcTestDataProcessor.ObtainTransactionData(genSrcData, testDataCfg, groupTypesCfg);

            var dateFilter = DateFilterCfg.Of(_transactions.First().DateStamp.AddDays(10), _transactions.Last().DateStamp, true);

            var transactions = _transactions.ByDateRange(groupTypesCfg, dateFilter).ToArray();

            foreach (var transaction in transactions)
            {
                var dateRange = new TransactionDateRange(dateFilter.StartDate, dateFilter.EndDate, transaction.DateStamp);
                Assert.InRange(dateRange.CurrentSpan, dateRange.StartSpan, dateRange.EndSpan);
            }
        }


    }
}