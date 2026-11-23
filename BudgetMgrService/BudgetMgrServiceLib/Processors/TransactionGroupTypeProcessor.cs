using System;
using System.Collections.Generic;
using System.Linq;
using BudgetMgrServiceLib.Models;
using Optional;

namespace BudgetMgrServiceLib.Processors
{
    public static class TransactionGroupTypeProcessor
    {
        public readonly static long DefaultId = -1;
        public readonly static long RemovedGroupId = -2;

        public static readonly string MiscTransGroupType = "misc";
        public static readonly string IncomeTransGroupType = "income";
        public static readonly string ExpenseTransGroupType = "expense";

        public static readonly TransGroupType[] DefaultTransGroupTypes = [
            new TransGroupType(1, IncomeTransGroupType),
            new TransGroupType(2, ExpenseTransGroupType)
        ];

        public static readonly string BankCreditTransType = "BANK CREDIT";
        public static readonly string DirectDebitTransType = "DIRECT DEBIT";
        public static readonly string AtmWithdrawalTransType = "ATM WITHDRAWAL";
        public static readonly string CashReceiptTransType = "CASH RECEIPT";
        public static readonly string VisaPurchaseTransType = "VISA PURCHASE";
        public static readonly string StandingOrderTransType = "STANDING ORDER";
        public static readonly string UnPaidTransactionFeeTransType = "UNPAID TRANSACTION FEE";
        public static readonly string UnknownTransType = "";

        public static readonly TransType[] DefaultTransTypes = [
            new TransType(1, BankCreditTransType),
            new TransType(2, DirectDebitTransType),
            new TransType(3, AtmWithdrawalTransType),
            new TransType(4, CashReceiptTransType),
            new TransType(5, VisaPurchaseTransType),
            new TransType(6, StandingOrderTransType),
            new TransType(7, UnPaidTransactionFeeTransType),
            new TransType(8, UnknownTransType)
        ];

        public static readonly CreditTypeRef[] DefaultCreditTypeRefs = [
            new CreditTypeRef(1, 1)
        ];

        public static readonly DebitTypeRef[] DefaultDebitTypeRefs = [
            new DebitTypeRef(1, 2),
            new DebitTypeRef(2, 3),
            new DebitTypeRef(3, 4),
            new DebitTypeRef(4, 5),
            new DebitTypeRef(5, 6),
            new DebitTypeRef(6, 7),
            new DebitTypeRef(7, 8)
        ];

        private static Option<IEnumerable<TransactionGroupEx>> DefaultTransGroup = Option.None<IEnumerable<TransactionGroupEx>>();

        private static Option<string> DefaultGroupName = Option.None<string>();

        private static Option<IEnumerable<Transaction>> DefaultTransactionSet = Option.None<IEnumerable<Transaction>>();

        public static GroupUpdateSourceReferences CreateUpdateSourceReferences(Option<IEnumerable<TransName>> transNamesToAdd,
                                                                               Option<IEnumerable<long>> nameRefsToRemove,
                                                                               Option<IEnumerable<TransactionNameEx>> nameRefsToAdd,
                                                                               Option<IEnumerable<TransactionGroupEx>> transGroupsToAdd)

        {
            return new GroupUpdateSourceReferences(transGroupsToAdd, transNamesToAdd, nameRefsToRemove, nameRefsToAdd);
        }

        public static UpdatedGroupSources CreateUpdatedGroupSources(Option<IEnumerable<TransName>> transNames,
                                                                    IEnumerable<TransactionNameEx> nameRefs,
                                                                    IEnumerable<TransactionGroupEx> transGroups)

        {
            return new UpdatedGroupSources(nameRefs,transNames,transGroups);
        }

        public static IEnumerable<TransactionGroupCfg> ToNameGroupsToBeUpdated(this IEnumerable<TransactionGroupCfg> srcGroupTypes, IEnumerable<TransactionGroupCfg> dbSetGroupTypes)
        {
            return (dbSetGroupTypes.Count() > 0)
                    ? srcGroupTypes.Where(group => dbSetGroupTypes.Any(_group => group.Id == _group.Id)).ToArray()
                    : srcGroupTypes.ToArray();
        }


        public static IEnumerable<TransactionGroupCfg> ToGroupTypesToBeAdded(this IEnumerable<TransactionGroupCfg> srcGroupTypes, IEnumerable<TransactionGroupCfg> dbSetGroupTypes)
        {
            var _groupTypesToBeAdded = (dbSetGroupTypes.Count() > 0)
                                        ? srcGroupTypes.Where(group => dbSetGroupTypes.Any(_group => group.Id == _group.Id) == false)
                                        : srcGroupTypes.ToArray();
            return dbSetGroupTypes.GenerateUniqueIds(_groupTypesToBeAdded, (groupType) => (int)groupType.Id)
                                  .Zip(_groupTypesToBeAdded, (id, group) => group with { Id = id });
        }

        public static IEnumerable<TransactionGroupCfg> ToGroupTypes(this IEnumerable<TransactionGroupCfg> nameGroups)
        {
            var _groups = nameGroups.ToArray();
            var ids = _groups.GenerateUniqueIds<TransactionGroupCfg, TransactionGroupCfg>();
            return ids.Zip(_groups, (id, group) => (group with { Id = id })).ToArray();
        }

        public static TransName ToTransName(this string name, long id) => new TransName(id,name);

        public static IEnumerable<string> ObtainTransNames(this IEnumerable<TransactionGroupCfg> nameSrc)
        {
            return nameSrc.Select(src => src.Names.Select(name => name))
                          .Flatten()
                          .Distinct();
        }

        public static IEnumerable<long> GenTransNamesToRemove(this IEnumerable<TransactionGroupCfg> removeSrc, GroupTypesCfgEx groupTypesCfg)
        {
            var transNamesToRemove = removeSrc.ObtainTransNames().ToArray();
            return groupTypesCfg.TransNames.Where(transName => transNamesToRemove.Any(name => name == transName.Name) == false)
                                .Select(transName => transName.Id)
                                .ToArray();
        }

        public static IEnumerable<TransName> GenTransNamesToAdd(this IEnumerable<TransactionGroupCfg> addSrc, GroupTypesCfgEx groupTypesCfg)
        {
            var transNamesToAdd = addSrc.ObtainTransNames()
                                        .Where(name => groupTypesCfg.TransNames.Any(transName => name == transName.Name) == false)
                                        .ToArray();

            return groupTypesCfg.TransNames.GenerateUniqueIds(transNamesToAdd, (transName) => (int)transName.Id)
                                .Zip(transNamesToAdd, (id, name) => name.ToTransName(id))
                                .ToArray();
        }

        public static IEnumerable<TransactionGroupEx> ObtainTransGroups(this IEnumerable<TransactionGroupCfg> transGroupSrc, GroupTypesCfgEx groupTypesCfg = null)
        {
            return transGroupSrc.Select(transGroup => new TransactionGroupEx(
                TransactionGroupTypeProcessor.DefaultId,
                transGroup.GroupName,
                groupTypesCfg.TransGroupTypes.First(groupType => groupType.Type == transGroup.GroupType).Id
            ));
        }

        public static IEnumerable<TransactionGroupEx> GenTransGroupsToRemove(this IEnumerable<TransactionGroupCfg> removeSrc, GroupTypesCfgEx groupTypesCfg)
        {
            var transGroupsToRemove = removeSrc.ObtainTransGroups(groupTypesCfg);

            return groupTypesCfg.TransGroups.GenerateUniqueIds(transGroupsToRemove, (transGroup) => (int)transGroup.Id)
                                .Zip(transGroupsToRemove, (id, transGroup) => transGroup with { Id = id })
                                .ToArray();
        }

        public static IEnumerable<TransactionGroupEx> GenTransGroupsToAdd(this IEnumerable<TransactionGroupCfg> addSrc, GroupTypesCfgEx groupTypesCfg)
        {
            var transGroupsToAdd = addSrc.ObtainTransGroups(groupTypesCfg)
                                         .Where(group => groupTypesCfg.TransGroups.Any(transGroup => group.GroupName == transGroup.GroupName) == false)
                                         .ToArray();


            return groupTypesCfg.TransGroups.GenerateUniqueIds(transGroupsToAdd, (transGroup) => (int)transGroup.Id)
                                .Zip(transGroupsToAdd, (id, transGroup) => transGroup with { Id = id })
                                .ToArray();
        }

        public static IEnumerable<long> GenNameRefsToRemove(this IEnumerable<TransactionGroupCfg> removeSrc, IEnumerable<TransName> transNames, GroupTypesCfgEx groupTypesCfg)
        {
            return removeSrc.Select(currSrc =>
            {
                return groupTypesCfg.NameRefs.Where(nameRef => nameRef.TransGroupFK == currSrc.Id)
                                    .Where(nameRef => currSrc.Names.Any(name => name == transNames.First(transName => nameRef.TransNameFK == transName.Id).Name) == false)
                                    .Select(nameRef => nameRef.Id);
            })
            .Flatten()
            .ToArray();
        }

        public static bool IsValidNameRef(this IEnumerable<TransName> transNames, TransactionNameEx nameRef, string name)
        {
            return (transNames.First(transName => nameRef.TransNameFK == transName.Id).Name == name);
        }

        public static IEnumerable<TransactionNameEx> ToNameRefsToAddFromGroup(this TransactionGroupCfg currGroup, IEnumerable<TransName> transNames, IEnumerable<TransactionNameEx> transNamesRefs)
        {
            return currGroup.Names.Where(name =>
            {
                return transNamesRefs.Where(nameRef => nameRef.TransGroupFK == currGroup.Id)
                                     .Any(nameRef => transNames.IsValidNameRef(nameRef, name)) == false;
            })
            .Select(name => new TransactionNameEx(0, currGroup.Id, transNames.First(transName => transName.Name == name).Id ));
        }

        public static IEnumerable<TransactionNameEx> GenNameRefsToAdd(this IEnumerable<TransactionGroupCfg> addSrc, IEnumerable<TransName> transNames, IEnumerable<TransactionNameEx> transNamesRefs)
        {

            var nameRefsToAdd = addSrc.Select(currSrc => currSrc.ToNameRefsToAddFromGroup(transNames, transNamesRefs))
                                      .Flatten();

            return transNamesRefs.GenerateUniqueIds(nameRefsToAdd, (transName) => (int)transName.Id)
                                 .Zip(nameRefsToAdd, (id, nameRefToAdd) => new TransactionNameEx(id, nameRefToAdd.TransGroupFK, nameRefToAdd.TransNameFK))
                                 .ToArray();
        }

        public static GroupTypesCfgEx UpdateGroupDataSource(this Tuple<GroupUpdateSourceReferences, UpdatedGroupSources> updateSourceReferences,
                                                            GroupTypesCfgEx groupTypesCfg)
        {
            return new GroupTypesCfgEx(
                groupTypesCfg.CreditTypesOfInterest.ToArray(),
                groupTypesCfg.DebitTypesOfInterest.ToArray(),
                updateSourceReferences.Item2.UpdatedTransNames.Match(some: transNames => transNames, none: () => new List<TransName>() { }),
                groupTypesCfg.TransTypes.ToArray(),
                groupTypesCfg.TransGroupTypes.ToArray(),
                updateSourceReferences.Item2.UpdatedTransGroups,
                updateSourceReferences.Item2.UpdatedNameRefs
            );
        }

        public static Tuple<GroupUpdateSourceReferences, UpdatedGroupSources> UpdateGroupDataWithDefault(this GroupTypesCfgEx groupTypesCfg, IEnumerable<TransactionGroupCfg> groupsToUpdate)
        {
            return groupTypesCfg.UpdateGroupData(groupsToUpdate, DefaultTransGroup);
        }

        public static Tuple<GroupUpdateSourceReferences, UpdatedGroupSources> UpdateGroupData(this GroupTypesCfgEx groupTypesCfg, IEnumerable<TransactionGroupCfg> groupsToUpdate, Option<IEnumerable<TransactionGroupEx>> transGroupsToAdd)
        {
            var _updatedTransGroups = transGroupsToAdd.ValueOr(groupTypesCfg.TransGroups.ToArray());

            var transNamesToAdd = Option.Some<IEnumerable<TransName>>(groupsToUpdate.GenTransNamesToAdd(groupTypesCfg));

            var updatedTransNames = transNamesToAdd.Map(namesToAdd => groupTypesCfg.TransNames.Concat(namesToAdd).ToArray());

            var nameRefsToRemove = updatedTransNames.Map(transNames => groupsToUpdate.GenNameRefsToRemove(transNames, groupTypesCfg));

            var nameRefsToAdd = updatedTransNames.Map(transNames => groupsToUpdate.GenNameRefsToAdd(transNames, groupTypesCfg.NameRefs));

            var unwrappedRefsToAdd = nameRefsToAdd.ValueOr(new List<TransactionNameEx>() { });
            var unwrappedRefsToRemove = nameRefsToRemove.ValueOr(new List<long>() { });

            var updatedNameRefs = groupTypesCfg.NameRefs.Where(nameRef => unwrappedRefsToRemove.Any(_nameRefId => _nameRefId == nameRef.Id) == false)
                                                        .Concat(unwrappedRefsToAdd)
                                                        .ToArray();
            return Tuple.Create(CreateUpdateSourceReferences(transNamesToAdd, nameRefsToRemove, nameRefsToAdd, transGroupsToAdd),
                                CreateUpdatedGroupSources(updatedTransNames.Map(transNames => transNames.AsEnumerable()), updatedNameRefs, _updatedTransGroups));
        }

        public static bool IsEmpty(this GroupTypesCfgEx groupTypesCfg)
        {
            return ((groupTypesCfg.CreditTypesOfInterest.Count() == 0) &&
                    (groupTypesCfg.DebitTypesOfInterest.Count() == 0) &&
                    (groupTypesCfg.TransGroupTypes.Count() == 0) &&
                    (groupTypesCfg.TransTypes.Count() == 0));
        }


        public static Tuple<GroupUpdateSourceReferences, UpdatedGroupSources> SyncGroupData(this GroupTypesCfgEx groupTypesCfg, IEnumerable<TransactionGroupCfg> groupsToUpdate)
        {
            groupsToUpdate = groupsToUpdate.Where(group => group.Id != TransactionGroupTypeProcessor.RemovedGroupId).ToArray();
            return groupTypesCfg.UpdateGroupData(groupsToUpdate, Option.Some(groupsToUpdate.GenTransGroupsToAdd(groupTypesCfg)));
        }

        public static TransactionGroupCfg ToUpdatedGroupWithDefault(this TransactionGroupCfg inputGroupType, IEnumerable<string> updatedNames)
        {
            return inputGroupType.ToUpdatedGroup(updatedNames, DefaultGroupName);
        }

        public static TransactionGroupCfg ToUpdatedGroup(this TransactionGroupCfg inputGroupType, IEnumerable<string> updatedNames, Option<string> groupName)
        {
            return new TransactionGroupCfg(inputGroupType.Id, groupName.ValueOr(inputGroupType.GroupName), inputGroupType.GroupType, updatedNames.ToArray());
        }

        public static TransactionGroupCfg ToTotalNameGroup(this IEnumerable<Transaction> transactions, string groupName, string groupType, GroupTypesCfgEx groupTypesCfg)
        {
            return new TransactionGroupCfg(groupTypesCfg.TransGroups.Count() + 1, groupName, groupType,transactions.ToNamesOfInterest(groupType, groupTypesCfg));
        }

        public static TransactionGroupCfg ToTotalIncomeGroup(this GroupTypesCfgEx groupTypesCfg, Option<IEnumerable<Transaction>> transactions)
        {
            var _transactions = transactions.ValueOr(Enumerable.Empty<Transaction>());
            return _transactions.ToTotalNameGroup("credits", IncomeTransGroupType, groupTypesCfg);
        }

        public static IEnumerable<string> ToCreditNamesOfInterestWithDefault(this GroupTypesCfgEx groupTypesCfg) => groupTypesCfg.ToCreditNamesOfInterest(DefaultTransactionSet);

        public static IEnumerable<string> ToCreditNamesOfInterest(this GroupTypesCfgEx groupTypesCfg, Option<IEnumerable<Transaction>> transactions)
        {
            return groupTypesCfg.ToTotalIncomeGroup(transactions).Names;
        }

        public static IEnumerable<string> ToDebitNamesOfInterestWithDefault(this GroupTypesCfgEx groupTypesCfg) => groupTypesCfg.ToDebitNamesOfInterest(DefaultTransactionSet);

        public static IEnumerable<string> ToDebitNamesOfInterest(this GroupTypesCfgEx groupTypesCfg, Option<IEnumerable<Transaction>> transactions)
        {
            return groupTypesCfg.ToTotalExpenseGroup(transactions).Names;
        }

        public static TransactionGroupCfg ToTotalExpenseGroup(this GroupTypesCfgEx groupTypesCfg, Option<IEnumerable<Transaction>> transactions)
        {
            var _transactions = transactions.ValueOr(Enumerable.Empty<Transaction>());
            return _transactions.ToTotalNameGroup("debits", ExpenseTransGroupType, groupTypesCfg);
        }

        public static TransactionGroupCfg ToExpenseGroup(this GroupTypesCfgEx groupTypesCfg, string groupName) => groupTypesCfg.ToTransNamesEx().First(group => group.GroupName == groupName);

        public static IEnumerable<TransactionName> ToTransNamesOfInterest(this GroupTypesCfgEx groupTypesCfg)
        {
            Func<string, IEnumerable<string>> genStartId = (groupType) => (groupType == IncomeTransGroupType) ? Enumerable.Empty<string>() : groupTypesCfg.ToCreditNamesOfInterestWithDefault();
            Func<string, IEnumerable<string>, IEnumerable<TransactionName>> toNamesOfInterest = (type, namesOfInterest) =>
            {
                var _id = 0;
                return genStartId(type).GenerateUniqueIds(namesOfInterest, (name) => _id++)
                                       .Zip(namesOfInterest, (id, nameOfInterest) => new TransactionName(id, type, nameOfInterest));
            };

            return toNamesOfInterest(IncomeTransGroupType, groupTypesCfg.ToCreditNamesOfInterestWithDefault())
                                                            .Concat(toNamesOfInterest(ExpenseTransGroupType, groupTypesCfg.ToDebitNamesOfInterestWithDefault()));
        }

        public static IEnumerable<String> FromTransNamesOfInterest(this IEnumerable<TransactionName> namesOfInterest, string groupType)
        {
            return namesOfInterest.Where(nameOfInterest => nameOfInterest.TransType == groupType)
                                  .Select(nameOfInterest => nameOfInterest.Name);
        }

        public static IEnumerable<TransactionType> ToTransTypesOfInterest(this GroupTypesCfgEx groupTypesCfg)
        {
            Func<string, IEnumerable<string>> genStartId = (groupType) => (groupType == IncomeTransGroupType) ? (new List<string>() { IncomeTransGroupType }) : groupTypesCfg.ToCreditTypesOfInterestEx();
            Func<string, IEnumerable<string>, IEnumerable<TransactionType>> toTypesOfInterest = (type, typesOfInterest) =>
            {
                var _id = 0;
                return genStartId(type).GenerateUniqueIds(typesOfInterest, (_type) => _id++)
                                       .Zip(typesOfInterest, (id, typeOfInterest) => new TransactionType(id, type, typeOfInterest));
            };

            return toTypesOfInterest(IncomeTransGroupType, groupTypesCfg.ToCreditTypesOfInterestEx()).Concat(toTypesOfInterest(ExpenseTransGroupType, groupTypesCfg.ToDebitTypesOfInterestEx()));
        }

        public static IEnumerable<String> FromTransTypesOfInterest(this IEnumerable<TransactionType> typesOfInterest, string groupType)
        {
            return typesOfInterest.Where(nameOfInterest => nameOfInterest.TransType == groupType)
                                  .Select(nameOfInterest => nameOfInterest.TypeName);
        }


        public static IEnumerable<string> ToMiscTypesOfInterest(this IEnumerable<Transaction> transactions, GroupTypesCfgEx groupTypesCfg)
        {
            var typesOfInterest = groupTypesCfg.ToCreditTypesOfInterestEx().Concat(groupTypesCfg.ToDebitTypesOfInterestEx());

            return transactions.Where(currTrans => typesOfInterest.Any(typeOfInterest => typeOfInterest == currTrans.TransType) == false)
                                                                  .Select(currTrans => currTrans.TransType)
                                                                  .Distinct();
        }


        public static IEnumerable<string> ToConfiguredNamesOfInterest(
            this GroupTypesCfgEx groupTypesCfg,
            string groupType)
        {
            return groupTypesCfg.ToTransNamesEx()
                                .Where(transName => transName.GroupType == groupType)
                                .Select(transName => transName.Names)
                                .Flatten();
        }

        public static IEnumerable<string> ToDescriptions(
            this IEnumerable<Transaction> transactions)
        {
            return transactions.Select(transaction => transaction.Description)
                               .Distinct();
        }

        public static IEnumerable<string> ExcludeDescriptions(
            this IEnumerable<Transaction> transactions,
            IEnumerable<string> descriptions)
        {
            return transactions.Where(transaction => descriptions.Contains(transaction.Description) == false)
                               .Select(transaction => transaction.Description);
        }

        public static IEnumerable<string> ToMiscNamesOfInterest(
            this IEnumerable<Transaction> transactions,
            string groupType,
            GroupTypesCfgEx groupTypesCfg)
        {
            return transactions.ExcludeDescriptions(groupTypesCfg.ToConfiguredNamesOfInterest(groupType))
                               .Distinct();
        }


        public static IEnumerable<string> ToKnownNamesOfInterest(
            this IEnumerable<Transaction> transactions,
            string groupType,
            GroupTypesCfgEx groupTypesCfg)
        {
            return transactions.ToDescriptions().Distinct();
        }

        public static IEnumerable<string> ToMiscNamesOfInterestFromCfg(this GroupTypesCfgEx groupTypesCfg)
        {
            Func<TransactionNameEx, TransName, bool> hasMatchingNameRef = (nameRef, transName) => nameRef.TransNameFK == transName.Id;

            return groupTypesCfg.TransNames
                                .Where(transName => groupTypesCfg.NameRefs.Any(nameRef => hasMatchingNameRef(nameRef, transName)) == false)
                                .Select(transName => transName.Name);
        }


        public static IEnumerable<string> ToTransNamesOfInterestEx(this GroupTypesCfgEx groupTypesCfg, string groupType)
        {
            var _transactions = Enumerable.Empty<Transaction>();

            return groupType.Match()
                            .With(_groupType => _groupType == MiscTransGroupType, (_groupType) => groupTypesCfg.ToMiscNamesOfInterestFromCfg())
                            .Else(_groupType => _transactions.ToNamesOfInterest(_groupType, groupTypesCfg))
                            .Do()
                            .ToArray();

        }
        public static IEnumerable<string> ToNamesOfInterest(this IEnumerable<Transaction> transactions, string groupType, GroupTypesCfgEx groupTypesCfg)
        {
            var typesOfInterest = (groupType == IncomeTransGroupType) ? groupTypesCfg.ToCreditTypesOfInterestEx() : groupTypesCfg.ToDebitTypesOfInterestEx();

            Func<IEnumerable<string>> namesFromTransactions = () => transactions.Where(currTrans => typesOfInterest.Any(typeOfInterest => typeOfInterest == currTrans.TransType))
                                                                                .Select(currTrans => currTrans.Description)
                                                                                .Distinct();

            Func<IEnumerable<string>> namesFromAppCfg = () => groupTypesCfg.ToTransNamesEx().Where(transName => transName.GroupType == groupType)
                                                                                            .Select(transName => transName.Names)
                                                                                            .Flatten();

            transactions.Where(currTrans => typesOfInterest.Any(typeOfInterest => typeOfInterest == currTrans.TransType))
                                                                                .Select(currTrans => currTrans.Description)
                                                                                .Distinct();
            return namesFromAppCfg().Concat(namesFromTransactions());
        }


        public static TransactionGroupCfg FromGroupTypeEx(this TransactionGroupEx groupTypeEx, GroupTypesCfgEx groupTypesCfg)
        {
            var groupTypeVal = groupTypesCfg.TransGroupTypes.First(transGroupType => transGroupType.Id == groupTypeEx.GroupTypeFK).Type;

            Func<long, string> toTransNameVal = (fk) => groupTypesCfg.TransNames.First(TransName => TransName.Id == fk).Name;

            var names = groupTypesCfg.NameRefs.Where(transNameRef => transNameRef.TransGroupFK == groupTypeEx.Id)
                                     .Select(transNameRef => toTransNameVal(transNameRef.TransNameFK))
                                     .ToArray();

            return new TransactionGroupCfg(groupTypeEx.Id, groupTypeEx.GroupName, groupTypeVal,names);
        }

        public static IEnumerable<TransactionGroupCfg> ToTransNamesEx(this GroupTypesCfgEx groupTypesCfg)
        {
            return groupTypesCfg.TransGroups.Select(transGroup => transGroup.FromGroupTypeEx(groupTypesCfg));
        }

        public static IEnumerable<string> ToCreditTypesOfInterestEx(this GroupTypesCfgEx groupTypesCfg)
        {
            return groupTypesCfg.CreditTypesOfInterest.Select(creditTypeRef => groupTypesCfg.TransTypes.First(transType => creditTypeRef.TransTypeFK == transType.Id).Type);
        }
        public static IEnumerable<string> ToDebitTypesOfInterestEx(this GroupTypesCfgEx groupTypesCfg)
        {
            return groupTypesCfg.DebitTypesOfInterest.Select(creditTypeRef => groupTypesCfg.TransTypes.First(transType => creditTypeRef.TransTypeFK == transType.Id).Type);
        }
        public static IEnumerable<string> ToTransTypesEx(this GroupTypesCfgEx groupTypesCfg)
        {
            return groupTypesCfg.TransTypes.Select(transType => transType.Type);
        }

        public static void WriteDataToFile(this GroupTypesCfgEx groupTypesCfg, string contentFilePath, string cfgFileName)
        {
            groupTypesCfg.GroupTypesCfgExToFile(cfgFileName, contentFilePath);
        }

    }
}
