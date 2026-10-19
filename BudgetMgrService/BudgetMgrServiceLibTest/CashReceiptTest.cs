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
// CashReceiptGenerator.GenCashReceiptData
// SrcTestDataProcessor.ObtainCsvData
// TransactionGroupTypeProcessor.ToDebitNamesOfInterestWithDefault
// SrcTestDataProcessor.NumericValueIsValid

namespace BudgetMgrServiceLibTest
{
    public class CashReceiptTest
    {
        public static Func<string, DateTime> toDateStamp = (val) => DateTime.Parse(val);
        public static Func<string, IEnumerable<string>> genCSVEntries = (row) => row.Split(new char[] { ',' }).ToArray();

        public static readonly TestDataCfg testDataCfg = AppCfgProcessor.ToTestDataCfg();
        public static readonly GroupTypesCfgEx groupTypesCfg = AppCfgProcessor.ToGroupTypesCfgEx();
        public static Func<TestDataCfg, GroupTypesCfgEx, DateTime, Random, SrcData> genSrcData = CashReceiptGenerator.GenCashReceiptData;

        public static IEnumerable<object[]> ObtainCSVRows()
        {
            var csvRows = SrcTestDataProcessor.ObtainCsvData(genSrcData, testDataCfg, groupTypesCfg);
            for (int i = 0; i < csvRows.Count(); i++)
            {
                yield return new object[] { csvRows.ElementAt(i) };
            }
        }

        // Rows have the right number of elements (4)
        [Theory]
        [MemberData(nameof(CashReceiptTest.ObtainCSVRows))]
        public void Test1_RowsHaveCorrectNumberOfElements(string rowData)
        {
            var elements = genCSVEntries(rowData);
            Assert.True(elements.Count() == 4);
        }

        //Dates are valid (DateTime.Parse("dd-MMM-yyyy") on first val in entry should return true)
        [Theory]
        [MemberData(nameof(CashReceiptTest.ObtainCSVRows))]
        public void Test2_DatesAreValid(string rowData)
        {
            var element = genCSVEntries(rowData).First();
            Assert.True(toDateStamp(element) != null);
        }

        // Desc values are valid (second val in entry should exist in range of known descriptions)
        [Theory]
        [MemberData(nameof(CashReceiptTest.ObtainCSVRows))]
        public void Test3_DescValuesAreValid(string rowData)
        {
            var element = genCSVEntries(rowData).Skip(1).First();
            Assert.Contains(element, groupTypesCfg.ToDebitNamesOfInterestWithDefault());
        }


        // Total values are valid floating point numbers (Double.Parse() on third val in entry should return true)
        [Theory]
        [MemberData(nameof(CashReceiptTest.ObtainCSVRows))]
        public void Test4_TotalValuesAreValid(string rowData)
        {
            var element = genCSVEntries(rowData).Skip(2).First();
            Assert.True(element.NumericValueIsValid());
        }

        // Cash paid values are valid floating point numbers (Double.Parse() on fourth val in entry should return true)
        [Theory]
        [MemberData(nameof(CashReceiptTest.ObtainCSVRows))]
        public void Test5_CashPaidValuesAreValid(string rowData)
        {
            var element = genCSVEntries(rowData).Skip(3).First();
            Assert.True(element.NumericValueIsValid());
        }
    }
}