using BudgetMgrServiceLib.Generators;
using BudgetMgrServiceLib.Models;
using BudgetMgrServiceLib.Processors;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Optional;

namespace BudgetMgrWebService.Processors
{
    public static class TransactionContextProcessor
    {

        public static void SeedWithData(this TransactionContext context, string contentFilePath)
        {
            context.Database.EnsureCreated();

            if (context.TransGroups.Any())
            {
                return;
            }

            var id = context.ContentFilePath.Count() + 1;
            var filePathDB = new FilePath(id, contentFilePath);

            var groupTypesEx = AppCfgProcessor.ToGroupTypesCfgEx(contentFilePath);
            context.ContentFilePath.Add(filePathDB);
            context.CreditTypesOfInterest.AddRange(groupTypesEx.CreditTypesOfInterest);
            context.DebitTypesOfInterest.AddRange(groupTypesEx.DebitTypesOfInterest);
            context.TransNames.AddRange(groupTypesEx.TransNames);
            context.TransTypes.AddRange(groupTypesEx.TransTypes);
            context.TransGroupTypes.AddRange(groupTypesEx.TransGroupTypes);
            context.TransGroups.AddRange(groupTypesEx.TransGroups);
            context.NameRefs.AddRange(groupTypesEx.NameRefs);

            // The follwing is a temporary solution to seed the database with transactions.
            // This should be via the post SrcData endpoint moving forward, which will be called by the front end to seed the database with transactions.
            // Once that is implemented, we can move this code and the testdatasettings.json file usage to the unit testing in the test project for the BudgetMgrWebService project.
            //{
            //    var testDataCfg = AppCfgProcessor.ToTestDataCfg(contentFilePath);
            //    Func<TestDataCfg, GroupTypesCfgEx, DateTime, Random, SrcData> genSrcData = StatementGenerator.GenStatementData;
            //    var transactions = SrcTestDataProcessor.ObtainTransactionData(genSrcData, testDataCfg, groupTypesEx).ToArray();
            //    context.Transactions.AddRange(transactions);
            //}

            context.SaveChanges();
        }

        public static IEnumerable<Transaction> FilterByDateRange(this IEnumerable<Transaction> _srcData, string startDate, string endDate)
        {
            var _startDate = DateTime.Parse(startDate);
            var _endDate = DateTime.Parse(endDate);

            return _srcData.Where((transaction) => ((transaction.DateStamp.CompareTo(_startDate) >= 0) && (transaction.DateStamp.CompareTo(_endDate) <= 0)));
        }

    }
}

