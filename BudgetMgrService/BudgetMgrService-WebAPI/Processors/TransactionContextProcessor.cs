using System;
using System.Collections.Generic;
using System.Linq;
using BudgetMgrServiceLib.Models;
using BudgetMgrServiceLib.Processors;
using BudgetMgrService_WebAPI.Models;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace BudgetMgrService_WebAPI.Processors
{
    public static class TransactionContextProcessor
    {
        public static Transaction ApplyUniqueId(Transaction transaction, int _Id) => Transaction.Of(_Id, transaction.DateStamp, transaction.TransType, transaction.Description, transaction.Amount);

        public static IEnumerable<Transaction> ApplyUniqueIds(this TransactionContext context, IEnumerable<Transaction> transactions)
        {
            var ids = context.Transactions.GenerateUniqueIds(transactions, (trans) => (int)trans.Id).ToArray();
            return transactions.Zip(ids, ApplyUniqueId);
        }

        public static FilePath ToFilePathDB(this string filePath, TransactionContext context)
        {
            var id = context.ContentFilePath.Count() + 1;
            return new FilePath()
            {
                Id = id,
                Value = filePath
            };
        }

        public static void SeedWithData(this TransactionContext context, string contentFilePath)
        {
            context.Database.EnsureCreated();

            if (context.TransGroups.Any())
            {
                return;
            }

            var groupTypesEx = AppCfgProcessor.ToGroupTypesCfgEx(contentFilePath);
            context.ContentFilePath.Add(contentFilePath.ToFilePathDB(context));
            context.CreditTypesOfInterest.AddRange(groupTypesEx.CreditTypesOfInterest);
            context.DebitTypesOfInterest.AddRange(groupTypesEx.DebitTypesOfInterest);
            context.TransNames.AddRange(groupTypesEx.TransNames);
            context.TransTypes.AddRange(groupTypesEx.TransTypes);
            context.TransGroupTypes.AddRange(groupTypesEx.TransGroupTypes);
            context.TransGroups.AddRange(groupTypesEx.TransGroups);
            context.NameRefs.AddRange(groupTypesEx.NameRefs);

            context.SaveChanges();
        }

        public static void WriteDataToFile(this TransactionContext context, string contentFilePath, string cfgFileName)
        {
            if (!context.TransGroups.Any())
            {
                return;
            }


            context.ToGroupTypeCfgEx().GroupTypesCfgExToFile(cfgFileName, contentFilePath);
        }

        public static Tuple<GroupUpdateSourceReferences, UpdatedGroupSources> AddGroupData(this TransactionContext dbContext, IEnumerable<TransactionGroupCfg> groupsToAdd)
        {
            var groupTypesCfg = dbContext.ToGroupTypeCfgEx();

            return groupTypesCfg.UpdateGroupData(groupsToAdd, Option.Some(groupsToAdd.GenTransGroupsToAdd(groupTypesCfg)));
        }

        public static void UpdateGroupDataSource(this Tuple<GroupUpdateSourceReferences, UpdatedGroupSources> updateSourceReferences,
                                                 TransactionContext dbContext)
        {
            updateSourceReferences.Item1.TransGroupsToAdd.MatchSome(groupsToAdd => dbContext.TransGroups.AddRange(groupsToAdd));

            updateSourceReferences.Item1.TransNamesToAdd.MatchSome(namesToAdd => dbContext.TransNames.AddRange(namesToAdd));

            updateSourceReferences.Item1.NameRefsToRemove.MatchSome(nameRefIds =>
            {
                nameRefIds.ForEach(nameRefId =>
                {
                    var nameRefToRemove = dbContext.NameRefs.FirstOrDefault(_nameRef => _nameRef.Id == nameRefId);
                    dbContext.NameRefs.Remove(nameRefToRemove);
                });
            });

            updateSourceReferences.Item1.NameRefsToAdd.MatchSome(refsToAdd => dbContext.NameRefs.AddRange(refsToAdd));
        }

        public static GroupTypesCfgEx ToGroupTypeCfgEx(this TransactionContext dbContext)
        {
            return new GroupTypesCfgEx()
            {
                CreditTypesOfInterest = dbContext.CreditTypesOfInterest.ToArray(),
                DebitTypesOfInterest = dbContext.DebitTypesOfInterest.ToArray(),
                TransNames = dbContext.TransNames.Select(transName => transName.Of(transName.Name.ToUpper())).ToArray(),
                TransTypes = dbContext.TransTypes.ToArray(),
                TransGroupTypes = dbContext.TransGroupTypes.ToArray(),
                TransGroups = dbContext.TransGroups.ToArray(),
                NameRefs = dbContext.NameRefs.ToArray()
            };
        }

        public static IEnumerable<Transaction> RemoveDuplicates(this IEnumerable<Transaction> _srcData, DbSet<Transaction> ctxTransactions)
        {
            var allTransactions = Option.Some<IEnumerable<Transaction>>(ctxTransactions.ToArray())
                                        .ValueOr(Enumerable.Empty<Transaction>());

            return _srcData.Where(lhs =>
            {
                return allTransactions.Any(rhs =>
                {
                    return ((lhs.DateStamp.CompareTo(rhs.DateStamp) == 0) &&
                            (lhs.TransType == rhs.TransType) &&
                            (lhs.Description == rhs.Description) &&
                            (lhs.Amount == rhs.Amount));
                }) == false;
            });
        }

        public static IEnumerable<Transaction> FilterByDateRange(this IEnumerable<Transaction> _srcData, string startDate, string endDate)
        {
            var _startDate = DateTime.Parse(startDate);
            var _endDate = DateTime.Parse(endDate);

            return _srcData.Where((transaction) => ((transaction.DateStamp.CompareTo(_startDate) >= 0) && (transaction.DateStamp.CompareTo(_endDate) <= 0)));
        }

        public static IEnumerable<string> ToMiscTransNamesOfInterest(this IEnumerable<Transaction> _srcData, TransactionContent content)
        {
            return _srcData.Where(currTransaction => {
                return (content.IncomeGroupNames.Any(name => name == currTransaction.Description) == false);
            })
            .ToMiscNamesOfInterest("expense", content.GroupTypeCfg);     
        }         
    }

}

