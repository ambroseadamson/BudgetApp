using BudgetMgrServiceLib.Generators;
using BudgetMgrServiceLib.Models;
using BudgetMgrServiceLib.Processors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Optional;

namespace BudgetMgrWebService.Processors
{
    public static class TransactionContextProcessor
    {
        // TODO: expose the following group types via the web service API, so that the front end
        // can obtain the relevant transactions based upon those types from the back end.
        private static readonly string miscellaenousGroupType = "misc";
        private static readonly string expenseGroupType = "expense";


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

            // TODO: this is a temporary solution to seed the database with transactions.
            // In the future, we will need to implement a more robust solution for seeding the database with transactions.
            // This should be via the post SrcData endpoint, which will be called by the front end to seed the database with transactions.
            // Once that is implemented, we can move this code and the testdatasettings.json file usage to the unit testing in the test project for the BudgetMgrWebService project.
            //{
            //    var testDataCfg = AppCfgProcessor.ToTestDataCfg(contentFilePath);
            //    Func<TestDataCfg, GroupTypesCfgEx, DateTime, Random, SrcData> genSrcData = StatementGenerator.GenStatementData;
            //    var transactions = SrcTestDataProcessor.ObtainTransactionData(genSrcData, testDataCfg, groupTypesEx).ToArray();
            //    context.Transactions.AddRange(transactions);
            //}

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
                    // TODO: replace this with an optional match to avoid null reference exception
                    if (nameRefToRemove != null)
                    {
                        dbContext.NameRefs.Remove(nameRefToRemove);
                    }
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

        public static IEnumerable<Transaction> ExcludeIncomeTransactions(
            this IEnumerable<Transaction> transactions,
            IEnumerable<string> incomeGroupNames)
        {
            return transactions.Where(transaction =>
                incomeGroupNames.Contains(transaction.Description) == false);
        }

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


        private static ActionResult<SummaryResult> CreateSummaryResult(TransactionContent content,
                                                                       IEnumerable<Transaction> transactions,
                                                                       string groupType)
        {
            var materializedTransactions = transactions.ToArray();

            return materializedTransactions.Length == 0
                ? null!
                : SummaryResult.Of(content, materializedTransactions, groupType);
        }

        public static Task<ActionResult<SummaryResult>> ToSummaryResultAsync(this TransactionContext _context, string? groupType = null)
        {
            var content = TransactionContent.Of(_context);
            return Task.FromResult(CreateSummaryResult(content, content.Transactions, groupType ?? expenseGroupType));
        }

        public static Task<ActionResult<SummaryResult>> FindSummaryResultAsync(this TransactionContext _context, string startDate, string endDate)
        {
            var content = TransactionContent.Of(_context);
            var transactions = content.Transactions.FilterByDateRange(startDate, endDate);

            return Task.FromResult(CreateSummaryResult(content, transactions, expenseGroupType));
        }

        public static Task<ActionResult<SummaryResult>> FindSummaryResultAsync(this TransactionContext _context, string startDate, string endDate, string groupType)
        {
            var content = TransactionContent.Of(_context);
            var transactions = content.Transactions.FilterByDateRange(startDate, endDate);

            return Task.FromResult(CreateSummaryResult(content, transactions, groupType));
        }

        private static ActionResult<TransactionSummary> CreateTransactionSummary(
            TransactionContent content,
            IEnumerable<Transaction> transactions,
            string groupType)
        {
            var materializedTransactions = transactions.ToArray();
            if (materializedTransactions.Length == 0)
            {
                return null!;
            }

            IEnumerable<string> expenditureGroupNames = groupType switch
            {
                var type when type == expenseGroupType => materializedTransactions.ToKnownTransNamesOfInterest(content),
                _ => materializedTransactions.ToMiscTransNamesOfInterest(content),
            };

            return materializedTransactions.GenTransactionSummaryData(
                content.IncomeGroupNames,
                expenditureGroupNames);
        }

        public static Task<ActionResult<TransactionSummary>> FindTransactionSummaryAsync(this TransactionContext _context)
        {
            var content = TransactionContent.Of(_context);
            return Task.FromResult(CreateTransactionSummary(content, content.Transactions, expenseGroupType));
        }

        public static Task<ActionResult<TransactionSummary>> FindTransactionSummaryAsync(this TransactionContext _context, string startDate, string endDate)
        {
            var content = TransactionContent.Of(_context);
            var transactions = content.Transactions.FilterByDateRange(startDate, endDate);
            return Task.FromResult(CreateTransactionSummary(content, transactions, expenseGroupType));
        }

        public static Task<ActionResult<TransactionSummary>> FindTransactionSummaryAsync(this TransactionContext _context, string groupType, string startDate, string endDate)
        {
            var content = TransactionContent.Of(_context);
            var transactions = content.Transactions.FilterByDateRange(startDate, endDate);
            return Task.FromResult(CreateTransactionSummary(content, transactions, groupType));
        }

        //public static Task<IEnumerable<TransactionGroupCfg>> ToListAsync(this TransactionContext _context)
        //{
        //    var content = TransactionContent.Of(_context);
        //    var _groups = content.GroupTypeCfg.ToTransNamesEx();
        //    var miscGroup = new TransactionGroupCfg()
        //    {
        //        Id = _groups.GenerateUniqueId(cfg => (int)cfg.Id),
        //        GroupName = "misc",
        //        GroupType = "expense",
        //        Names = content.Transactions.ToMiscTransNamesOfInterest(content)
        //    }.AsCollection();
        //    var transactionGroupCfgs = _groups.Concat(miscGroup).ToArray();

        //    return Task.FromResult<IEnumerable<TransactionGroupCfg>>(transactionGroupCfgs);
        //}


        public static Task<TransactionGroupCfg> FindTransactionGroupCfgAsync(this TransactionContext _context, UpdateTransactionName transactionName)
        {
            var groupOfInterest = _context.ToGroupTypeCfgEx()
                              .ToTransNamesEx()
                              .FirstOrDefault(group => group.Id == transactionName.Id);

            var transactionGroupCfg = (groupOfInterest == null)
                                        ? null
                                        : new TransactionGroupCfg()
                                        {
                                            Id = groupOfInterest.Id,
                                            GroupName = groupOfInterest.GroupName,
                                            GroupType = groupOfInterest.GroupType,
                                            Names = groupOfInterest.Names
                                        };

            return Task.FromResult<TransactionGroupCfg>(transactionGroupCfg);
        }

        public static async Task<string> CommitChangesAsync(this TransactionContext _context, TransactionGroupCfg groupCfg)
        {
            _context.TransactionGroupCfg.Add(groupCfg);
            await _context.SaveChangesAsync();

            _context.WriteDataToFile(_context.ContentFilePath.First().Value, AppCfgProcessor.DefaultGroupTypeCfgFileName);

            return string.Join(",", _context.ToGroupTypeCfgEx().ToTransNamesEx().Select(group => group.GroupName));
        }

        public static void UpdateTransactionGroupCfg(this TransactionContext _context, TransactionGroupCfg groupCfg)
        {
            // update existing entries in the groupType Data set...
            var groupsToUpdate = groupCfg.AsCollection().ToArray();

            var groupTypesCfg = _context.ToGroupTypeCfgEx();
            groupTypesCfg.UpdateGroupDataWithDefault(groupsToUpdate).UpdateGroupDataSource(_context);
        }

        public static void RemoveTransactionGroupCfg(this TransactionContext _context, TransactionGroupCfg groupCfg)
        {
            UpdateTransactionGroupCfg(_context, groupCfg);
        }

    }

}

