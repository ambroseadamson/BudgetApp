using System.Collections.Generic;
using System.Linq;
using Xunit;
using BudgetMgrServiceLib.Models;
using BudgetMgrServiceLib.Processors;
using System;
using BudgetMgrServiceLib.Generators;
using System.IO;
using Optional;

// Extensions methods used in the tests:
// AppCfgProcessor.ToTestDataCfg
// AppCfgProcessor.ToGroupTypesCfgEx
// StatementGenerator.GenCashReceiptData
// SrcTestDataProcessor.ObtainCsvData
// SrcTestDataProcessor.ObtainTransactionData
// TransactionGroupTypeProcessor.ToDebitNamesOfInterestWithDefault
// TransactionGroupTypeProcessor.ToDebitTypesOfInterestEx
// TransactionGroupTypeProcessor.ToTransTypesEx
// TransactionGroupTypeProcessor.ToGroupTypesToBeAdded
// SrcTestDataProcessor.NumericValueIsValid

namespace BudgetMgrServiceLibTest
{
    public class TransactionGroupTypeTests
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

        public static readonly string MiscTransactionDescription = "*unknown payee*";

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

        public static IEnumerable<object[]> ObtainAddGroupTypeSet()
        {
            var currentSet = new List<TransactionGroupCfg>();
            var outputAddSet = groupTypesCfg.ToTransNamesEx().ToGroupTypesToBeAdded(currentSet).ToArray();
            for (int i = 0; i < outputAddSet.Count(); i++)
            {
                yield return new object[] { currentSet, outputAddSet.ElementAt(i) };
            }
        }

        // Add group types test by using existing set from appCfg configuration on empty set and compare results.
        [Theory]
        [MemberData(nameof(ObtainAddGroupTypeSet))]
        public void Test1_AddAppCfgEntriesToEmptySet(List<TransactionGroupCfg> currentSet, TransactionGroupCfg addedGroupType)
        {
            //       Current Set = Empty
            //       Added Group Type = Name Group from app cfg as Group Type
            Assert.DoesNotContain(currentSet, (currentGroupType) => currentGroupType.GroupName == addedGroupType.GroupName);
        }

        public static IEnumerable<object[]> ObtainUpdatedGroupTypeSet()
        {
            var currentSet = groupTypesCfg.ToTransNamesEx().ToList();
            var outputUpdatedSet = groupTypesCfg.ToTransNamesEx()
                                                .Select(currGroup => currGroup.ToUpdatedGroupWithDefault(currGroup.Names.Take(1).ToArray()))
                                                .ToNameGroupsToBeUpdated(currentSet)
                                                .ToArray();

            for (int i = 0; i < outputUpdatedSet.Count(); i++)
            {
                yield return new object[] { currentSet, outputUpdatedSet.ElementAt(i) };
            }
        }

        // Update test by creating a set from the app configuration which limits each entry to a maximum of two names on a populated set and compare results.
        [Theory]
        [MemberData(nameof(ObtainUpdatedGroupTypeSet))]

        public void Test2_UpdateWithModAppCfgEntriesToExistingSet(List<TransactionGroupCfg> currentSet, TransactionGroupCfg updatedGroupType)
        {
            //       Current Set = Name Groups from app cfg as Group Types
            //       Updated Group Type = Name Group from app cfg as Group Type where max name set = 2
            Assert.Contains(currentSet, (currentGroupType) => currentGroupType.GroupName == updatedGroupType.GroupName);
            Assert.True(updatedGroupType.Names.Count() <= 2);
        }


        // Persist update test test by creating a set from the app configuration which limits each entry to a single name on a populated set
        // compare updated set to persisted set to see if they match.
        [Fact]
        public void Test3_PersistedUpdateWithModAppCfgEntriesToExistingSet()
        {
            Func<GroupTypesCfgEx> genUpdatedCfgData = () =>
            {
                var currentSet = groupTypesCfg.ToTransNamesEx().ToArray();

                var transGroups = groupTypesCfg.ToTransNamesEx()
                                               .Select(currGroup => currGroup.ToUpdatedGroupWithDefault(currGroup.Names.Take(1).ToArray()))
                                               .ToNameGroupsToBeUpdated(currentSet)
                                               .ToArray();

                return groupTypesCfg.UpdateGroupDataWithDefault(transGroups).UpdateGroupDataSource(groupTypesCfg);
            };

            var originalUpdatedCfg = genUpdatedCfgData();

            var persistedUpdate = originalUpdatedCfg.GroupTypesCfgExToJsonData();

            var reloadedUpdateCfg = persistedUpdate.GroupTypesCfgExFromJsonData();

            FunctionalComparer<TransactionGroupCfg> transGroupTypeComparer = new FunctionalComparer<TransactionGroupCfg>()
            {
                comparer = (x, y) => ((x.GroupName == y.GroupName) && (x.GroupType == y.GroupType) && (x.Names.SequenceEqual(y.Names))),
                hashCodeGenerator = (x) => (x.GroupName.GetHashCode() + x.GroupType.GetHashCode() + x.Names.GetHashCode())
            };

            FunctionalComparer<CreditTypeRef> creditTypeRefComparer = new FunctionalComparer<CreditTypeRef>()
            {
                comparer = (x, y) => ((x.Id == y.Id) && (x.TransTypeFK == y.TransTypeFK)),
                hashCodeGenerator = (x) => (x.Id.GetHashCode() + x.TransTypeFK.GetHashCode())
            };

            FunctionalComparer<DebitTypeRef> debitTypeRefComparer = new FunctionalComparer<DebitTypeRef>()
            {
                comparer = (x, y) => ((x.Id == y.Id) && (x.TransTypeFK == y.TransTypeFK)),
                hashCodeGenerator = (x) => (x.Id.GetHashCode() + x.TransTypeFK.GetHashCode())
            };

            FunctionalComparer<TransType> transTypeComparer = new FunctionalComparer<TransType>()
            {
                comparer = (x, y) => ((x.Id == y.Id) && (x.Type == y.Type)),
                hashCodeGenerator = (x) => (x.Id.GetHashCode() + x.Type.GetHashCode())
            };

            Assert.True(originalUpdatedCfg.ToTransNamesEx().SequenceEqual(reloadedUpdateCfg.ToTransNamesEx(), transGroupTypeComparer));

            Assert.True(originalUpdatedCfg.CreditTypesOfInterest.SequenceEqual(reloadedUpdateCfg.CreditTypesOfInterest, creditTypeRefComparer));
            Assert.True(originalUpdatedCfg.DebitTypesOfInterest.SequenceEqual(reloadedUpdateCfg.DebitTypesOfInterest, debitTypeRefComparer));
            Assert.True(originalUpdatedCfg.TransTypes.SequenceEqual(reloadedUpdateCfg.TransTypes, transTypeComparer));
        }

        public static IEnumerable<object[]> ObtainAddedAndUpdatedGroupTypeSet()
        {
            var currentSet = groupTypesCfg.ToTransNamesEx().ToList();

            var indexes = Enumerable.Range(currentSet.Count() + 1, currentSet.Count()).ToArray();

            Func<long, TransactionGroupCfg, IEnumerable<string>, Option<string>, TransactionGroupCfg> fnToAddedGroup = (id, inputGroupType, updatedNames, groupName) =>
            {
                return new TransactionGroupCfg()
                {
                    Id = id,
                    GroupType = inputGroupType.GroupType,
                    GroupName = groupName.ValueOr(inputGroupType.GroupName),
                    Names = updatedNames.ToArray()
                };
            };

            var outputAddedSet = groupTypesCfg.ToTransNamesEx()
                                              .Zip(indexes, (currGroup, index) => fnToAddedGroup(index, currGroup, currGroup.Names.Take(1).ToArray(), $"{currGroup.GroupName}{index}".Some()))
                                              .ToGroupTypesToBeAdded(currentSet)
                                              .ToArray();
            Assert.True(outputAddedSet.Count() > 0);

            var outputUpdatedSet = groupTypesCfg.ToTransNamesEx()
                                                .Select(currGroup => currGroup.ToUpdatedGroupWithDefault(currGroup.Names.Take(1).ToArray()))
                                                .ToNameGroupsToBeUpdated(currentSet)
                                                .ToGroupTypes()
                                                .ToArray();
            Assert.True(outputUpdatedSet.Count() > 0);

            var outputSet = outputAddedSet.Zip(outputUpdatedSet, (add, update) => new { GroupTypeToAdd = add, GroupTypeToUpdate = update })
                                          .ToArray();
            Assert.True(outputSet.Count() > 0);

            for (int i = 0; i < outputSet.Count(); i++)
            {
                yield return new object[] {
                    currentSet,
                    outputSet.ElementAt(i).GroupTypeToAdd,
                    outputSet.ElementAt(i).GroupTypeToUpdate
                };
            }
        }

        // Add and update test by creating a set by generating a randomised set of names from half the set from the app configuration and 3 additional entries with randomised sets of names and compare results.
        [Theory]
        [MemberData(nameof(ObtainAddedAndUpdatedGroupTypeSet))]

        public void Test4_AddAndUpdateWithModAppCfgEntriesToExistingSet(List<TransactionGroupCfg> currentSet, TransactionGroupCfg addedGroupType, TransactionGroupCfg updatedGroupType)
        {
            //       Input Set = Name Groups from app cfg with randomised set of names from half the set from the app configuration and 3 additional entries with randomised sets of names
            //       Current Set = Name Groups from app cfg as Group Types
            //       Expected Set = Name Groups from app cfg as Group Types with randomised set of names from half the set from the app configuration and 3 additional entries with randomised sets of names

            Assert.Contains(currentSet, (currentGroupType) => currentGroupType.GroupName == updatedGroupType.GroupName);
            Assert.True(updatedGroupType.Names.Count() <= 2);

            Assert.DoesNotContain(currentSet, (currentGroupType) => currentGroupType.GroupName == addedGroupType.GroupName);
            Assert.True(addedGroupType.Names.Count() <= 2);
        }

        // Add a test where miscellaenous trans types are detected from transactions when ToMiscTypesOfInterest() is called
        [Theory]
        [MemberData(nameof(ObtainTransactions))]

        public void Test5_ObtainMiscTransactionTypes(Transaction currTrans)
        {
            var miscTypes = currTrans.AsCollection()
                                     .ToMiscTypesOfInterest(groupTypesCfg)
                                     .ToArray();

            Assert.True(miscTypes.Count() > 0);
            Assert.True(miscTypes.All((miscType) => isMiscValue(miscType) == true));
        }


        // Add a test where miscellaenous trans names are detected from transactions when ToMiscNamesOfInterest() is called
        [Theory]
        [MemberData(nameof(ObtainTransactions))]

        public void Test6_ObtainMiscExpenseTransactionNames(Transaction currTrans)
        {
            var miscNames = currTrans.AsCollection()
                                     .ToMiscNamesOfInterest("expense", groupTypesCfg)
                                     .ToArray();

            Assert.True(miscNames.Count() > 0);
            Assert.True(miscNames.All((miscName) => isMiscValue(miscName) == true));
        }

        [Theory]
        [MemberData(nameof(ObtainTransactions))]

        public void Test7_ObtainMiscIncomeTransactionNames(Transaction currTrans)
        {
            var miscNames = currTrans.AsCollection()
                                     .ToMiscNamesOfInterest("income", groupTypesCfg)
                                     .ToArray();

            Assert.True(miscNames.Count() > 0);
            Assert.True(miscNames.All((miscName) => isMiscValue(miscName) == true));
        }

        // Add a test where miscellaenous trans names are detected from transactions when ToMiscNamesOfInterest() is called
        [Fact]
        public void Test8_ObtainAllMiscTransactionNames()
        {
            Func<GroupTypesCfgEx> genModifiedCfgData = () =>
            {
                Func<IEnumerable<TransName>> genMiscNames = () =>
                {
                    var aMiscName = MiscTransactionDescription.AsCollection()
                                                              .ToArray();

                    return groupTypesCfg.TransNames
                                        .GenerateUniqueIds(aMiscName, (transName) => (int)transName.Id)
                                        .Zip(aMiscName, (id, name) => name.ToTransName(id))
                                        .ToArray();
                };

                return new GroupTypesCfgEx()
                {
                    CreditTypesOfInterest = groupTypesCfg.CreditTypesOfInterest.ToArray(),
                    DebitTypesOfInterest = groupTypesCfg.DebitTypesOfInterest.ToArray(),
                    TransTypes = groupTypesCfg.TransTypes.ToArray(),
                    TransGroupTypes = groupTypesCfg.TransGroupTypes.ToArray(),
                    TransGroups = groupTypesCfg.TransGroups.ToArray(),

                    TransNames = genMiscNames().ToArray(),
                    NameRefs = groupTypesCfg.NameRefs.ToArray()
                };
            };

            var miscNames = genModifiedCfgData().ToMiscNamesOfInterestFromCfg()
                                                .ToArray();

            Func<string, bool> _isMiscValue = (miscValue) => miscValue == MiscTransactionDescription;
            Assert.True(miscNames.Count() == 1);
            Assert.True(miscNames.All((miscName) => _isMiscValue(miscName) == true));
        }
    }
}