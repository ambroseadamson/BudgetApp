using BudgetMgrServiceLib.Models;
using BudgetMgrServiceLib.Processors;
using Microsoft.AspNetCore.Mvc;
using Optional;

namespace BudgetMgrWebService.Processors
{
    public static class TransactionGroupTypeCtlProcessor
    {
        // TODO: expose the following group types via the web service API, so that the front end
        // can obtain the relevant transactions based upon those types from the back end.

        private static readonly string miscellaenousGroupType = "misc";
        private static readonly string expenseGroupType = "expense";

        public static IEnumerable<string> ToMiscTransNamesOfInterest(
            this IEnumerable<Transaction> transactions,
            TransactionContent content) =>
            transactions.ExcludeIncomeTransactions(content.IncomeGroupNames)
                        .ToMiscNamesOfInterest(expenseGroupType, content.GroupTypeCfg);

        public static IEnumerable<string> ToKnownTransNamesOfInterest(
            this IEnumerable<Transaction> transactions,
            TransactionContent content) =>
            transactions.ExcludeIncomeTransactions(content.IncomeGroupNames)
                        .ToKnownNamesOfInterest(expenseGroupType, content.GroupTypeCfg);


        public static Tuple<GroupUpdateSourceReferences, UpdatedGroupSources> AddGroupData(this TransactionContext dbContext, IEnumerable<TransactionGroupCfg> groupsToAdd)
        {
            var groupTypesCfg = dbContext.ToGroupTypeCfgEx();

            return groupTypesCfg.UpdateGroupData(groupsToAdd, Option.Some(groupsToAdd.GenTransGroupsToAdd(groupTypesCfg)));
        }

        public static Tuple<GroupUpdateSourceReferences, UpdatedGroupSources> RemoveGroupData(this TransactionContext dbContext, IEnumerable<TransactionGroupCfg> groupsToRemove)
        {
            var groupTypesCfg = dbContext.ToGroupTypeCfgEx();

            return groupTypesCfg.UpdateGroupData(groupsToRemove, Option.Some(groupsToRemove.GenTransGroupsToRemove(groupTypesCfg)));
        }

        public static GroupTypesCfgEx ToGroupTypeCfgEx(this TransactionContext dbContext)
        {
            var fnTransNameToUpper = (TransName transName) => transName with { Name = transName.Name.ToUpper() };
            return new GroupTypesCfgEx(
                dbContext.CreditTypesOfInterest.ToArray(),
                dbContext.DebitTypesOfInterest.ToArray(),
                dbContext.TransNames.Select(fnTransNameToUpper).ToArray(),
                dbContext.TransTypes.ToArray(),
                dbContext.TransGroupTypes.ToArray(),
                dbContext.TransGroups.ToArray(),
                dbContext.NameRefs.ToArray()
            );
        }

        public static Task<ActionResult<IEnumerable<TransactionGroupCfg>>> ToTransactionGroupCfgsAsync(this TransactionContext _context)
        {
            var content = TransactionContent.Of(_context);
            var _groups = content.GroupTypeCfg.ToTransNamesEx();
            var miscGroup = new TransactionGroupCfg(
                _groups.GenerateUniqueId(cfg => (int)cfg.Id),
                "misc",
                "expense",
                content.Transactions.ToMiscTransNamesOfInterest(content)
            ).AsCollection();
            var transactionGroupCfgs = _groups.Concat(miscGroup).ToArray();

            return (transactionGroupCfgs.Length == 0)
                ? Task.FromResult<ActionResult<IEnumerable<TransactionGroupCfg>>>(new NotFoundResult())
                : Task.FromResult<ActionResult<IEnumerable<TransactionGroupCfg>>>(transactionGroupCfgs);
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
                    // TODO: replace this with an optional match to avoid null reference exception
                    if (nameRefToRemove != null)
                    {
                        dbContext.NameRefs.Remove(nameRefToRemove);
                    }
                });
            });

            updateSourceReferences.Item1.NameRefsToAdd.MatchSome(refsToAdd => dbContext.NameRefs.AddRange(refsToAdd));
        }

        public static IEnumerable<TransactionGroupCfg> CollateAddedGroupCfgs(this IEnumerable<TransactionGroupCfg> updatedGroupCfgs)
        {
            // NOTE: Misc group type is not persisted to the database,
            //       as it used to reflect the transactions that are not part of the known group types,
            //       and is used to provide a summary of those transactions.
            // TODO: Also at the momment, you cannot remove a group type from the database,
            //       as this is not fully implemented at this time.

            if (updatedGroupCfgs.Any(g => g.GroupName == miscellaenousGroupType) == false)
            {
                return updatedGroupCfgs;
            }

            var addedTransGroupStartIndex = updatedGroupCfgs.Where(g => g.GroupName == miscellaenousGroupType).FirstOrDefault()?.Id - 1;
            var _updatedGroupCfgs = updatedGroupCfgs.Where(g => g.GroupName != miscellaenousGroupType).ToArray();

            return _updatedGroupCfgs.Select(g => {
                if (g.Id == g.DefaultId() && (addedTransGroupStartIndex != null))
                {
                    addedTransGroupStartIndex = addedTransGroupStartIndex + 1;
                    return g with { Id = addedTransGroupStartIndex.Value };
                }
                return g;
            }).ToArray();
        }

        public static void UpdateTransactionGroupCfg(this TransactionContext _context, IEnumerable<TransactionGroupCfg> updatedGroupCfgs)
        {
            // persist the groupCfg (which contains the COMPLETE updated data set) to the database, and update the relevant group data source with the new groupCfg data.
            // TODO: Also at the momment, you cannot remove a group type from the database,
            //       as this is not fully implemented at this time.
            var srcGroupTypesCfgs = _context.ToGroupTypeCfgEx();
            srcGroupTypesCfgs.AddGroupData(updatedGroupCfgs.CollateAddedGroupCfgs()).UpdateGroupDataSource(_context);
        }
    }
}
