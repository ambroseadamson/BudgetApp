using BudgetMgrServiceLib.Models;
using BudgetMgrServiceLib.Generators;
using BudgetMgrServiceLib.Processors;
using Optional;

namespace BudgetMgrWebService.Processors
{
    public static class DataStoreProcessor
    {

        public static void SeedWithData(this DataStore dataStore, string contentFilePath)
        {
            dataStore.Database.EnsureCreated();

            if (dataStore.TransGroups.Any())
            {
                return;
            }

            var id = dataStore.ContentFilePath.Count() + 1;
            var filePathDB = new FilePath(id, contentFilePath);

            var groupTypesEx = AppCfgProcessor.ToGroupTypesCfgEx(contentFilePath);
            dataStore.ContentFilePath.Add(filePathDB);
            dataStore.CreditTypesOfInterest.AddRange(groupTypesEx.CreditTypesOfInterest);
            dataStore.DebitTypesOfInterest.AddRange(groupTypesEx.DebitTypesOfInterest);
            dataStore.TransNames.AddRange(groupTypesEx.TransNames);
            dataStore.TransTypes.AddRange(groupTypesEx.TransTypes);
            dataStore.TransGroupTypes.AddRange(groupTypesEx.TransGroupTypes);
            dataStore.TransGroups.AddRange(groupTypesEx.TransGroups);
            dataStore.NameRefs.AddRange(groupTypesEx.NameRefs);

            // The follwing is a temporary solution to seed the database with transactions.
            // This should be via the post SrcData endpoint moving forward, which will be called by the front end to seed the database with transactions.
            // Once that is implemented, we can move this code and the testdatasettings.json file usage to the unit testing in the test project for the BudgetMgrWebService project.
            //{
            //    var testDataCfg = AppCfgProcessor.ToTestDataCfg(contentFilePath);
            //    Func<TestDataCfg, GroupTypesCfgEx, DateTime, Random, SrcData> genSrcData = StatementGenerator.GenStatementData;
            //    var transactions = SrcTestDataProcessor.ObtainTransactionData(genSrcData, testDataCfg, groupTypesEx).ToArray();
            //    dataStore.Transactions.AddRange(transactions);
            //}

            dataStore.SaveChanges();
        }

        public static IEnumerable<Transaction> FilterByDateRange(this IEnumerable<Transaction> _srcData, string startDate, string endDate)
        {
            var _startDate = DateTime.Parse(startDate);
            var _endDate = DateTime.Parse(endDate);

            return _srcData.Where((transaction) => ((transaction.DateStamp.CompareTo(_startDate) >= 0) && (transaction.DateStamp.CompareTo(_endDate) <= 0)));
        }

        public static DateFilter CreateDateFilter(this IEnumerable<Transaction> transactions)
        {
            var sortedTransactions = transactions.OrderBy(transaction => transaction.DateStamp);
            return new DateFilter()
            {
                StartDate = sortedTransactions.First().DateStamp,
                EndDate = sortedTransactions.Last().DateStamp
            };
        }

        public static TransactionContent CreateTransactionContnent(this DataStore dbContext)
        {
            var groupTypeCfg = dbContext.ToGroupTypeCfgEx();
            var transactions = Option.Some<IEnumerable<Transaction>>(dbContext.Transactions.ToArray());


            var incomeGroupNames = groupTypeCfg.ToTotalIncomeGroup(transactions).Names.ToArray();

            return new TransactionContent(
                groupTypeCfg,
                transactions.ValueOr(Enumerable.Empty<Transaction>()),
                incomeGroupNames
            );
        }

        public static SummaryResult CreateSummaryResult(this TransactionContent content, IEnumerable<Transaction> allTransactions, string groupType)
        {
            Func<IEnumerable<string>> fnObtainGroupNames = () => content.GroupTypeCfg.ToTransNamesEx()
                                                                                     .Select(group => group.GroupName)
                                                                                     .Where(groupName => groupName != "credits")
                                                                                     .Concat(groupType.AsCollection())
                                                                                     .ToArray();

            var miscTransNamesOfInterest = allTransactions.ToMiscTransNamesOfInterest(content);

            Func<string, TransactionSummaryEntry> fnGenTransData = (_groupType) =>
            {

                var _expenditureGroupNames = (_groupType == groupType) ? miscTransNamesOfInterest : content.GroupTypeCfg.ToExpenseGroup(_groupType).Names.ToArray();

                return new TransactionSummaryEntry(0, _groupType, allTransactions.GenTransactionSummaryData(content.IncomeGroupNames, _expenditureGroupNames).ExpSummaryPct);
            };

            Func<string, TransactionCreditDetails> fnGenIncomeData = (_groupType) =>
            {
                var _expenditureGroupNames = content.GroupTypeCfg.ToExpenseGroup(_groupType).Names.ToArray();

                return allTransactions.GenTransactionSummaryData(content.IncomeGroupNames, _expenditureGroupNames).Income;
            };

            return new SummaryResult(
                0, 
                fnGenIncomeData(fnObtainGroupNames().First()), 
                allTransactions.CreateDateFilter(), 
                fnObtainGroupNames().Select(groupName => fnGenTransData(groupName)).ToArray()
            );
        }

    }
}

