using System;
using System.Collections.Generic;
using System.Linq;
using BudgetMgrServiceLib.Models;
using Optional;

namespace BudgetMgrServiceLib.Processors
{
    public static class TransactionGroupTypeProcessor
    {
        public static long DefaultId(this TransactionGroupCfg group) => -1;

        public static string MiscTransGroupType = "misc";

        private static Option<IEnumerable<TransactionGroupEx>> DefaultTransGroup = Option.None<IEnumerable<TransactionGroupEx>>();

        private static Option<string> DefaultGroupName = Option.None<string>();

        private static Option<IEnumerable<Transaction>> DefaultTransactionSet = Option.None<IEnumerable<Transaction>>();

        public static TransactionGroupCfg Of(this TransactionGroupCfg group, long id)
        {
            return new TransactionGroupCfg()
            {
                Id = id,
                GroupName = group.GroupName,
                GroupType = group.GroupType,
                Names = group.Names.ToArray()
            };
        }

        public static TransactionGroupEx Of(this TransactionGroupEx groupEx, long id)
        {
            return new TransactionGroupEx()
            {
                Id = id,
                GroupName = groupEx.GroupName,
                GroupTypeFK = groupEx.GroupTypeFK
            };
        }

        public static TransactionGroupEx To(this TransactionGroupCfg group, IEnumerable<TransGroupType> groupTypes, long id)
        {
            return new TransactionGroupEx()
            {
                Id = id,
                GroupName = group.GroupName,
                GroupTypeFK = groupTypes.First(groupType => groupType.Type == group.GroupType).Id
            };
        }

        public static GroupUpdateSourceReferences CreateUpdateSourceReferences(Option<IEnumerable<TransName>> transNamesToAdd,
                                                                               Option<IEnumerable<long>> nameRefsToRemove,
                                                                               Option<IEnumerable<TransactionNameEx>> nameRefsToAdd,
                                                                               Option<IEnumerable<TransactionGroupEx>> transGroupsToAdd)

        {
            return new GroupUpdateSourceReferences()
            {
                TransNamesToAdd = transNamesToAdd,
                NameRefsToRemove = nameRefsToRemove,
                NameRefsToAdd = nameRefsToAdd,
                TransGroupsToAdd = transGroupsToAdd
            };
        }

        public static UpdatedGroupSources CreateUpdatedGroupSources(Option<IEnumerable<TransName>> transNames,
                                                                    IEnumerable<TransactionNameEx> nameRefs,
                                                                    IEnumerable<TransactionGroupEx> transGroups)

        {
            return new UpdatedGroupSources()
            {
                UpdatedNameRefs = nameRefs,
                UpdatedTransNames = transNames,
                UpdatedTransGroups = transGroups
            };
        }

        public static TransactionName CreateTransName(long id, string transType, string name)
        {
            return new TransactionName()
            {
                Id = id,
                TransType = transType,
                Name = name
            };
        }

        public static TransName Of(this TransName transName, string name)
        {
            return new TransName()
            {
                Id = transName.Id,
                Name = name
            };
        }

        public static TransactionType CreateTransType(long id, string transType, string typeName)
        {
            return new TransactionType()
            {
                Id = id,
                TransType = transType,
                TypeName = typeName
            };
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
                                  .Zip(_groupTypesToBeAdded, (id, group) => group.Of(id));
        }

        public static IEnumerable<TransactionGroupCfg> ToGroupTypes(this IEnumerable<TransactionGroupCfg> nameGroups)
        {
            var _groups = nameGroups.ToArray();
            var ids = _groups.GenerateUniqueIds<TransactionGroupCfg, TransactionGroupCfg>();
            return ids.Zip(_groups, (id, group) => group.Of(id)).ToArray();
        }

        public static TransName ToTransName(this string name, long id)
        {
            return new TransName()
            {
                Id = id,
                Name = name
            };
        }

        public static IEnumerable<string> ObtainNewTransNames(this IEnumerable<TransactionGroupCfg> newNameSrc)
        {
            return newNameSrc.Select(src => src.Names.Select(name => name))
                             .Flatten()
                             .Distinct();
        }

        public static IEnumerable<long> GenTransNamesToRemove(this IEnumerable<TransactionGroupCfg> removeSrc, GroupTypesCfgEx groupTypesCfg)
        {
            var newTransNames = removeSrc.ObtainNewTransNames().ToArray();
            return groupTypesCfg.TransNames.Where(transName => newTransNames.Any(name => name == transName.Name) == false)
                                .Select(transName => transName.Id)
                                .ToArray();
        }

        public static IEnumerable<TransactionGroupEx> GenTransGroupsToAdd(this IEnumerable<TransactionGroupCfg> addSrc, GroupTypesCfgEx groupTypesCfg)
        {
            var transGroupsToAdd = addSrc.Select(transGroup => transGroup.To(groupTypesCfg.TransGroupTypes, transGroup.DefaultId()));

            return groupTypesCfg.TransGroups.GenerateUniqueIds(transGroupsToAdd, (transGroup) => (int)transGroup.Id)
                                .Zip(transGroupsToAdd, (id, transGroup) => transGroup.Of(id))
                                .ToArray();
        }


        public static IEnumerable<TransName> GenTransNamesToAdd(this IEnumerable<TransactionGroupCfg> addSrc, GroupTypesCfgEx groupTypesCfg)
        {
            var newTransNames = addSrc.ObtainNewTransNames()
                                      .Where(name => groupTypesCfg.TransNames.Any(transName => name == transName.Name) == false)
                                      .ToArray();

            return groupTypesCfg.TransNames.GenerateUniqueIds(newTransNames, (transName) => (int)transName.Id)
                                .Zip(newTransNames, (id, name) => name.ToTransName(id))
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
            .Select(name => new TransactionNameEx() { Id = 0, TransGroupFK = currGroup.Id, TransNameFK = transNames.First(transName => transName.Name == name).Id });
        }

        public static IEnumerable<TransactionNameEx> GenNameRefsToAdd(this IEnumerable<TransactionGroupCfg> addSrc, IEnumerable<TransName> transNames, IEnumerable<TransactionNameEx> transNamesRefs)
        {

            var nameRefsToAdd = addSrc.Select(currSrc => currSrc.ToNameRefsToAddFromGroup(transNames, transNamesRefs))
                                      .Flatten();

            return transNamesRefs.GenerateUniqueIds(nameRefsToAdd, (transName) => (int)transName.Id)
                                 .Zip(nameRefsToAdd, (id, nameRefToAdd) => new TransactionNameEx() { Id = id, TransGroupFK = nameRefToAdd.TransGroupFK, TransNameFK = nameRefToAdd.TransNameFK })
                                 .ToArray();
        }

        public static GroupTypesCfgEx UpdateGroupDataSource(this Tuple<GroupUpdateSourceReferences, UpdatedGroupSources> updateSourceReferences,
                                                            GroupTypesCfgEx groupTypesCfg)
        {
            return new GroupTypesCfgEx()
            {
                CreditTypesOfInterest = groupTypesCfg.CreditTypesOfInterest.ToArray(),
                DebitTypesOfInterest = groupTypesCfg.DebitTypesOfInterest.ToArray(),
                TransTypes = groupTypesCfg.TransTypes.ToArray(),
                TransGroupTypes = groupTypesCfg.TransGroupTypes.ToArray(),
                TransGroups = updateSourceReferences.Item2.UpdatedTransGroups,

                TransNames = updateSourceReferences.Item2.UpdatedTransNames.Match(some: transNames => transNames, none: () => new List<TransName>() { }),
                NameRefs = updateSourceReferences.Item2.UpdatedNameRefs
            };

        }

        public static Tuple<GroupUpdateSourceReferences, UpdatedGroupSources> UpdateGroupDataWithDefault(this GroupTypesCfgEx groupTypesCfg, IEnumerable<TransactionGroupCfg> groupsToUpdate)
        {
            return groupTypesCfg.UpdateGroupData(groupsToUpdate, DefaultTransGroup);
        }

        public static Tuple<GroupUpdateSourceReferences, UpdatedGroupSources> UpdateGroupData(this GroupTypesCfgEx groupTypesCfg, IEnumerable<TransactionGroupCfg> groupsToUpdate, Option<IEnumerable<TransactionGroupEx>> transGroupsToAdd)
        {
            var _updatedTransGroups = transGroupsToAdd.Match(some: groupsToAdd => groupTypesCfg.TransGroups.Concat(groupsToAdd).ToArray(),
                                                             none: () => groupTypesCfg.TransGroups.ToArray());

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

        public static Tuple<GroupUpdateSourceReferences, UpdatedGroupSources> AddGroupData(this GroupTypesCfgEx groupTypesCfg, IEnumerable<TransactionGroupCfg> groupsToAdd)
        {
            return groupTypesCfg.UpdateGroupData(groupsToAdd, Option.Some(groupsToAdd.GenTransGroupsToAdd(groupTypesCfg)));
        }


        public static TransactionGroupCfg ToUpdatedGroupWithDefault(this TransactionGroupCfg inputGroupType, IEnumerable<string> updatedNames)
        {
            return inputGroupType.ToUpdatedGroup(updatedNames, DefaultGroupName);
        }

        public static TransactionGroupCfg ToUpdatedGroup(this TransactionGroupCfg inputGroupType, IEnumerable<string> updatedNames, Option<string> groupName)
        {
            return new TransactionGroupCfg()
            {
                Id = inputGroupType.Id,
                GroupType = inputGroupType.GroupType,
                GroupName = groupName.ValueOr(inputGroupType.GroupName),
                Names = updatedNames.ToArray()
            };
        }

        public static TransactionGroupCfg ToTotalNameGroup(this IEnumerable<Transaction> transactions, string groupName, string groupType, GroupTypesCfgEx groupTypesCfg)
        {
            return new TransactionGroupCfg()
            {
                Id = groupTypesCfg.TransGroups.Count() + 1,
                GroupName = groupName,
                GroupType = groupType,
                Names = transactions.ToNamesOfInterest(groupType, groupTypesCfg)
            };
        }

        public static TransactionGroupCfg ToTotalIncomeGroup(this GroupTypesCfgEx groupTypesCfg, Option<IEnumerable<Transaction>> transactions)
        {
            var _transactions = transactions.ValueOr(Enumerable.Empty<Transaction>());
            return _transactions.ToTotalNameGroup("credits", "income", groupTypesCfg);
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
            return _transactions.ToTotalNameGroup("debits", "expense", groupTypesCfg);
        }

        public static TransactionGroupCfg ToExpenseGroup(this GroupTypesCfgEx groupTypesCfg, string groupName) => groupTypesCfg.ToTransNamesEx().First(group => group.GroupName == groupName);

        public static IEnumerable<TransactionName> ToTransNamesOfInterest(this GroupTypesCfgEx groupTypesCfg)
        {
            Func<string, IEnumerable<string>> genStartId = (groupType) => (groupType == "income") ? Enumerable.Empty<string>() : groupTypesCfg.ToCreditNamesOfInterestWithDefault();
            Func<string, IEnumerable<string>, IEnumerable<TransactionName>> toNamesOfInterest = (type, namesOfInterest) =>
            {
                var _id = 0;
                return genStartId(type).GenerateUniqueIds(namesOfInterest, (name) => _id++)
                                       .Zip(namesOfInterest, (id, nameOfInterest) => CreateTransName(id, type, nameOfInterest));
            };

            return toNamesOfInterest("income", groupTypesCfg.ToCreditNamesOfInterestWithDefault())
                                                            .Concat(toNamesOfInterest("expense", groupTypesCfg.ToDebitNamesOfInterestWithDefault()));
        }

        public static IEnumerable<String> FromTransNamesOfInterest(this IEnumerable<TransactionName> namesOfInterest, string groupType)
        {
            return namesOfInterest.Where(nameOfInterest => nameOfInterest.TransType == groupType)
                                  .Select(nameOfInterest => nameOfInterest.Name);
        }

        public static IEnumerable<TransactionType> ToTransTypesOfInterest(this GroupTypesCfgEx groupTypesCfg)
        {
            Func<string, IEnumerable<string>> genStartId = (groupType) => (groupType == "income") ? (new List<string>() { "income" }) : groupTypesCfg.ToCreditTypesOfInterestEx();
            Func<string, IEnumerable<string>, IEnumerable<TransactionType>> toTypesOfInterest = (type, typesOfInterest) =>
            {
                var _id = 0;
                return genStartId(type).GenerateUniqueIds(typesOfInterest, (_type) => _id++)
                                       .Zip(typesOfInterest, (id, typeOfInterest) => CreateTransType(id, type, typeOfInterest));
            };

            return toTypesOfInterest("income", groupTypesCfg.ToCreditTypesOfInterestEx()).Concat(toTypesOfInterest("expense", groupTypesCfg.ToDebitTypesOfInterestEx()));
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


        public static IEnumerable<string> ToMiscNamesOfInterest(this IEnumerable<Transaction> transactions, string groupType, GroupTypesCfgEx groupTypesCfg)
        {
            var namesOfInterest =  groupTypesCfg.ToTransNamesEx().Where(transName => transName.GroupType == groupType)
                                                                 .Select(transName => transName.Names)
                                                                 .Flatten();

            return transactions.Where(currTrans => namesOfInterest.Contains(currTrans.Description) == false)
                               .Select(currTrans => currTrans.Description)
                               .Distinct();
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
            var typesOfInterest = (groupType == "income") ? groupTypesCfg.ToCreditTypesOfInterestEx() : groupTypesCfg.ToDebitTypesOfInterestEx();

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

            return new TransactionGroupCfg()
            {
                Id = groupTypeEx.Id,
                GroupName = groupTypeEx.GroupName,
                GroupType = groupTypeVal,
                Names = names
            };
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
