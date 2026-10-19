using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using BudgetMgrServiceLib.Models;
using BudgetMgrServiceLib.Processors;
using BudgetMgrServiceLib.Generators;

// Extensions methods used in the tests:
// AppCfgProcessor.ToTestDataCfg
// AppCfgProcessor.ToGroupTypesCfgEx
// StatementGenerator.GenCashReceiptData
// SrcTestDataProcessor.ObtainCsvData
// TransactionGroupTypeProcessor.ToDebitNamesOfInterestWithDefault
// TransactionGroupTypeProcessor.ToTransTypesEx
// SrcTestDataProcessor.NumericValueIsValid


namespace BudgetMgrServiceLibTest
{
    public class StatementTest
    {
        public static Func<string, DateTime> toDateStamp = (val) => DateTime.Parse(val);
        public static Func<string, IEnumerable<string>> genCsvEntries = (row) => row.Replace("\"", "").Split(new char[] { ',' });

        public static readonly TestDataCfg testDataCfg = AppCfgProcessor.ToTestDataCfg();
        public static readonly GroupTypesCfgEx groupTypesCfg = AppCfgProcessor.ToGroupTypesCfgEx();
        public static Func<TestDataCfg, GroupTypesCfgEx, DateTime, Random, SrcData> genSrcData = StatementGenerator.GenStatementData;

        public static Func<string, string> trimUnwantedChars = (entry) => entry.Replace('�', ' ');

        public static IEnumerable<object[]> ObtainCsvRows()
        {
            var csvRows = SrcTestDataProcessor.ObtainCsvData(genSrcData, testDataCfg, groupTypesCfg).Skip(6);
            for (int i = 0; i < csvRows.Count(); i++)
            {
                yield return new object[] { trimUnwantedChars(csvRows.ElementAt(i)) };
            }
        }


        // Rows have the right number of elements (6)
        [Theory]
        [MemberData(nameof(StatementTest.ObtainCsvRows))]
        public void Test1_RowsHaveCorrectNumberOfElements(string rowData)
        {
            var elements = genCsvEntries(rowData);
            Assert.True(elements.Count() == 6);
        }

        // // Dates are valid (DateTime.Parse("dd MMM yyyy") on first val in entry should return true)
        [Theory]
        [MemberData(nameof(StatementTest.ObtainCsvRows))]
        public void Test2_DatesAreValid(string rowData)
        {
            var element = genCsvEntries(rowData).First();
            Assert.True(toDateStamp(element) != null);
        }

        // // Transaction types are values are valid (second val in entry should exist in range of known types)
        [Theory]
        [MemberData(nameof(StatementTest.ObtainCsvRows))]
        public void Test3_TransactionsValuesAreValid(string rowData)
        {
            var element = genCsvEntries(rowData).Skip(1).First();
            Assert.Contains(element, groupTypesCfg.ToTransTypesEx());
        }


        // // Description values are valid (third val in entry should exist in range of known descriptions)
        [Theory]
        [MemberData(nameof(StatementTest.ObtainCsvRows))]
        public void Test4_DescriptionValuesAreValid(string rowData)
        {
            var element = genCsvEntries(rowData).Skip(2).First();
            Assert.Contains(element, groupTypesCfg.ToDebitNamesOfInterestWithDefault());
        }

        // // Debit values are valid (fourth val in entry is empty or Double.Parse() on fourth val in entry should return true)
        [Theory]
        [MemberData(nameof(StatementTest.ObtainCsvRows))]
        public void Test5_DebitValuesAreValid(string rowData)
        {
            var element = genCsvEntries(rowData).Skip(3).First();
            Assert.True((element.Count() == 0) ? true : element.NumericValueIsValid());
        }

        // // Credit values are valid (fifth val in entry is empty or Double.Parse() on fifth val in entry should return true)
        [Theory]
        [MemberData(nameof(StatementTest.ObtainCsvRows))]
        public void Test6_CreditValuesAreValid(string rowData)
        {
            var element = genCsvEntries(rowData).Skip(4).First();
            Assert.True((element.Count() == 0) ? true : element.NumericValueIsValid());
        }


        // // Amount values are valid (Double.Parse() on sixth val in entry should return true)
        [Theory]
        [MemberData(nameof(StatementTest.ObtainCsvRows))]
        public void Test7_AmountValuesAreValid(string rowData)
        {
            var element = genCsvEntries(rowData).Skip(5).First();
            Assert.True(element.NumericValueIsValid());
        }

    }
}