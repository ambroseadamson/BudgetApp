using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using BudgetMgrServiceLib.Models;
using BudgetMgrServiceLib.Processors;
using BudgetMgrServiceLib.Generators;
using Optional;

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
// SummarySrcData.Of

namespace BudgetMgrServiceLibTest
{
    public class TransactionSummaryTests
    {

        public static readonly TestDataCfg testDataCfg = AppCfgProcessor.ToTestDataCfg();
        public static readonly GroupTypesCfgEx groupTypesCfg = AppCfgProcessor.ToGroupTypesCfgEx();

        public static readonly Transaction[] transactions = SrcTestDataProcessor.ObtainTransactionData(StatementGenerator.GenStatementData, testDataCfg, groupTypesCfg).ToArray();

        public static readonly TransactionGroupCfg totalIncomeGroup = groupTypesCfg.ToTotalIncomeGroup(Option.Some<IEnumerable<Transaction>>(transactions));

        public static readonly TransactionGroupCfg totalExpenditureGroup = groupTypesCfg.ToTotalExpenseGroup(Option.Some<IEnumerable<Transaction>>(transactions));

        public static IEnumerable<object[]> ObtainSummarySrcContent()
        {
            var expenditureGroupTypes = groupTypesCfg.ToTransNamesEx().Where(group => group.GroupType == "expense").ToArray();
            Func<TransactionGroupCfg, SummarySrcData> genSummarySrcData = (expGroupType) =>
            {
                return transactions.GenTransactionSummaryData(totalIncomeGroup.Names, expGroupType.Names)
                                   .ToSummarySrcData(expGroupType.GroupName);
            };

            Func<IEnumerable<TransactionGroupCfg>, int, object> genSummaryData = (_expenditureGroupTypes, i) =>
            {
                return genSummarySrcData((i == -1) ? totalExpenditureGroup : _expenditureGroupTypes.ElementAt(i));
            };

            for (int i = -1; i < expenditureGroupTypes.Count(); i++)
            {
                yield return new object[] { genSummaryData(expenditureGroupTypes, i) };
            }
        }

        public static IEnumerable<object[]> ObtainPositiveBalanceSummarySrcContent()
        {
            var incomeDetails = transactions.ToCreditDetails(totalIncomeGroup.Names);

            var expenditureGroupTypes = groupTypesCfg.ToTransNamesEx().Where(group => group.GroupType == "expense").ToArray();
            Func<TransactionGroupCfg, SummarySrcData> genSummarySrcData = (expGroupType) =>
            {
                return transactions.GenTransactionSummaryData(totalIncomeGroup.Names, expGroupType.Names)
                                   .ToSummarySrcData(expGroupType.GroupName);
            };

            incomeDetails.Total = expenditureGroupTypes.Concat(new List<TransactionGroupCfg>() { totalExpenditureGroup })
                                                       .Select(expenseGroupType => genSummarySrcData(expenseGroupType))
                                                       .Select(expenseSummary => expenseSummary.Total)
                                                       .Max() * 2;

            Func<IEnumerable<TransactionGroupCfg>, int, object> genSummaryData = (_expenditureGroupTypes, i) =>
            {
                return genSummarySrcData((i == -1) ? totalExpenditureGroup : _expenditureGroupTypes.ElementAt(i));
            };

            for (int i = -1; i < expenditureGroupTypes.Count(); i++)
            {
                yield return new object[] { genSummaryData(expenditureGroupTypes, i) };
            }
        }

        public static IEnumerable<object[]> ObtainNegativeBalanceSummarySrcContent()
        {
            var incomeDetails = transactions.ToCreditDetails(totalIncomeGroup.Names);

            var expenditureGroupTypes = groupTypesCfg.ToTransNamesEx().Where(group => group.GroupType == "expense").ToArray();
            Func<TransactionGroupCfg, SummarySrcData> genSummarySrcData = (expGroupType) =>
            {
                var expenditureDetails = transactions.ToCreditDetails(expGroupType.Names);
                return SummarySrcData.Of(expGroupType.GroupName,
                                         expenditureDetails.Total,
                                         incomeDetails.ToCreditBalance(expenditureDetails),
                                         incomeDetails.ToExpenseSummaryPct(expenditureDetails));
            };

            incomeDetails.Total = expenditureGroupTypes.Select(expenseGroupType => genSummarySrcData(expenseGroupType))
                                                       .Select(expenseSummary => expenseSummary.Total)
                                                       .Min() - 10;

            Func<IEnumerable<TransactionGroupCfg>, int, object> genSummaryData = (_expenditureGroupTypes, i) =>
            {
                return genSummarySrcData((i == -1) ? totalExpenditureGroup : _expenditureGroupTypes.ElementAt(i));
            };

            for (int i = -1; i < expenditureGroupTypes.Count(); i++)
            {
                yield return new object[] { genSummaryData(expenditureGroupTypes, i) };
            }
        }

        // Summaries from  Selected Group are correct (Summary for a given transaction group type, produces the expected total, balance and perecentage summary)
        [Theory]
        [MemberData(nameof(ObtainSummarySrcContent))]
        public void Test1_SummariesFromSelectedGroupAreCorrect(SummarySrcData summarySrc)
        {
            Func<TransactionGroupCfg, string, bool> isGroupOfInterest = (nameGroup, groupType) => (nameGroup.GroupName == groupType);

            var expenditureGroup = (summarySrc.ExpenditureGroupType == totalExpenditureGroup.GroupName)
                                    ? totalExpenditureGroup.Names
                                    : groupTypesCfg.ToTransNamesEx().First(transNames => isGroupOfInterest(transNames, summarySrc.ExpenditureGroupType)).Names;

            var summaryResult = transactions.GenTransactionSummaryData(totalIncomeGroup.Names, expenditureGroup)
                                            .ToSummarySrcData(summarySrc.ExpenditureGroupType);

            Assert.Equal(summarySrc.Total, summaryResult.Total);
            Assert.Equal(summarySrc.Balance, summaryResult.Balance);
            Assert.Equal(summarySrc.SummaryPct, summaryResult.SummaryPct);
        }

        // Positive balances from  Selected Group have correct criteria (If income is greater than expense, balance is greater than zero)
        [Theory]
        [MemberData(nameof(ObtainPositiveBalanceSummarySrcContent))]
        public void Test2_PositiveSummarybalancesHaveCorrectCriteria(SummarySrcData summarySrc)
        {
            Assert.True((summarySrc.Balance > 0));
        }

        //Negative balances from  Selected Group have correct criteria (If income is less than expense, balance is less than zero)
        [Theory]
        [MemberData(nameof(ObtainNegativeBalanceSummarySrcContent))]
        public void Test3_NegativeSummarybalancesHaveCorrectCriteria(SummarySrcData summarySrc)
        {
            Assert.True((summarySrc.Balance < 0));
        }

        //Summary Percentages from  Selected Group have correct criteria (percentage is between 0 and 100)
        [Theory]
        [MemberData(nameof(ObtainSummarySrcContent))]
        public void Test4_SummariesPercentagesFromSelectedGroupAreCorrect(SummarySrcData summarySrc)
        {
            Assert.InRange(summarySrc.SummaryPct, 0, 100);
        }
    }
}