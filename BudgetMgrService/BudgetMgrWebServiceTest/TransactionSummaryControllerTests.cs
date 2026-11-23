using BudgetMgrServiceLib.Generators;
using BudgetMgrServiceLib.Models;
using BudgetMgrServiceLib.Processors;
using BudgetMgrWebService.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Transactions;
using Xunit;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BudgetMgrWebServiceTest
{
    public class TransactionSummaryControllerTests
    {
        public static readonly TestDataCfg testDataCfg = AppCfgProcessor.ToTestDataCfg();
        public static readonly GroupTypesCfgEx groupTypesCfg = AppCfgProcessor.ToGroupTypesCfgEx();
        public static Func<TestDataCfg, GroupTypesCfgEx, DateTime, Random, SrcData> genSrcData = StatementGenerator.GenStatementData;

        private DataStore createDataStore()
        {
            var options = new DbContextOptionsBuilder<DataStore>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var dataStore = new DataStore(options);
            dataStore.CreditTypesOfInterest.AddRange(groupTypesCfg.CreditTypesOfInterest);
            dataStore.DebitTypesOfInterest.AddRange(groupTypesCfg.DebitTypesOfInterest);
            dataStore.TransNames.AddRange(groupTypesCfg.TransNames);
            dataStore.TransTypes.AddRange(groupTypesCfg.TransTypes);
            dataStore.TransGroupTypes.AddRange(groupTypesCfg.TransGroupTypes);
            dataStore.TransGroups.AddRange(groupTypesCfg.TransGroups);
            dataStore.NameRefs.AddRange(groupTypesCfg.NameRefs);
            return dataStore;
        }


        //Get data store of no transactions reports an error(Not Found)
        [Fact]
        public async Task Test1_Get_Data_Store_Of_No_Transactions_Reports_Not_Found_Error()
        {
            // Arrange
            var dataStore = createDataStore();
            dataStore.SaveChanges();

            var controller = new TransactionSummaryController(dataStore);

            //// Act
            var result = await controller.GetTransactionSummary();

            //// Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result.Result);
            Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        }

        //Get data store of transactions reports a summary based upon the transactions in the data store
        [Fact]
        public async Task Test2_Get_Data_Store_Of_Transactions_Reports_Summary_Based_Upon_Transactions_In_Data_Store()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<DataStore>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var dataStore = createDataStore();
            var transactions = SrcTestDataProcessor.ObtainTransactionData(genSrcData, testDataCfg, groupTypesCfg).ToArray();
            dataStore.Transactions.AddRange(transactions);
            dataStore.SaveChanges();

            var controller = new TransactionSummaryController(dataStore);

            //// Act
            var result = await controller.GetTransactionSummary();

            //// Assert
            var summaryResult = Assert.IsType<TransactionSummary>(result.Value);
            Assert.NotNull(summaryResult);
            Assert.NotEmpty(summaryResult.Expenditure.Transactions);
            Assert.True(summaryResult.Balance != 0);
            Assert.True(summaryResult.ExpSummaryPct != 0);
        }

        //Get with date range outside of transactions in data store reports an error(Not Found)
        [Fact]
        public async Task Test3_Get_With_Date_Range_Outside_Of_Transactions_Reports_Not_Found_Error()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<DataStore>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var dataStore = createDataStore();

            var transactions = SrcTestDataProcessor.ObtainTransactionData(genSrcData, testDataCfg, groupTypesCfg).ToArray();
            dataStore.Transactions.AddRange(transactions);
            dataStore.SaveChanges();

            var controller = new TransactionSummaryController(dataStore);

            //// Act
            var startDate = transactions.Last().DateStamp.AddDays(10);
            var endDate = startDate.AddDays(10);
            var dateRage = new DateFilterBodyParams(startDate.ToDateStampString(), endDate.ToDateStampString());
            var result = await controller.GetTransactionSummaryByDate(dateRage);

            //// Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result.Result);
            Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        }

        //Get with date range within transactions reports a summary based upon the transactions in the data store
        [Fact]
        public async Task Test4_Get_With_Date_Range_Within_Transactions_Reports_Summary_Based_Upon_Transactions_In_Data_Store()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<DataStore>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var dataStore = createDataStore();

            var transactions = SrcTestDataProcessor.ObtainTransactionData(genSrcData, testDataCfg, groupTypesCfg).ToArray();
            dataStore.Transactions.AddRange(transactions);
            dataStore.SaveChanges();

            var controller = new TransactionSummaryController(dataStore);

            //// Act
            var startDate = transactions.First().DateStamp;
            var endDate = transactions.Last().DateStamp;
            var dateRage = new DateFilterBodyParams(startDate.ToDateStampString(), endDate.ToDateStampString());
            var result = await controller.GetTransactionSummaryByDate(dateRage);

            //// Assert
            var summaryResult = Assert.IsType<TransactionSummary>(result.Value);
            Assert.NotNull(summaryResult);
            Assert.NotEmpty(summaryResult.Expenditure.Transactions);
            Assert.True(summaryResult.Balance != 0);
            Assert.True(summaryResult.ExpSummaryPct != 0);
        }

        //Get with date range outside of transactions in data store and invalid Transaction Group Type reports an error(Bad Request)
        [Fact]
        public async Task Test5_Get_With_Date_Range_Outside_Of_Range_And_Invalid_Transaction_Group_Type_Reports_Bad_Request_Error()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<DataStore>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var dataStore = createDataStore();

            var transactions = SrcTestDataProcessor.ObtainTransactionData(genSrcData, testDataCfg, groupTypesCfg).ToArray();
            dataStore.Transactions.AddRange(transactions);
            dataStore.SaveChanges();

            var controller = new TransactionSummaryController(dataStore);

            //// Act
            var startDate = transactions.Last().DateStamp.AddDays(10);
            var endDate = startDate.AddDays(10);
            var dateRangeAndGroup = new DateFilterAndGroupTypeBodyParams(startDate.ToDateStampString(), endDate.ToDateStampString(), "UNKNOWN");
            var result = await controller.GetTransactionSummaryByGroupTypeAndDate(dateRangeAndGroup);

            //// Assert
            var badRequestResult = Assert.IsType<BadRequestResult>(result.Result);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        }

        //Get with date range outside of transactions in data store and valid Transaction Group Type reports an error(Not Found)
        [Fact]
        public async Task Test6_Get_With_Date_Range_Outside_Of_Range_And_Valid_Transaction_Group_Type_Reports_Not_Found_Error()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<DataStore>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var dataStore = createDataStore();

            var transactions = SrcTestDataProcessor.ObtainTransactionData(genSrcData, testDataCfg, groupTypesCfg).ToArray();
            dataStore.Transactions.AddRange(transactions);
            dataStore.SaveChanges();

            var controller = new TransactionSummaryController(dataStore);

            //// Act
            var startDate = transactions.Last().DateStamp.AddDays(10);
            var endDate = startDate.AddDays(10);
            var dateRangeAndGroup = new DateFilterAndGroupTypeBodyParams(startDate.ToDateStampString(), endDate.ToDateStampString(), TransactionGroupTypeProcessor.ExpenseTransGroupType);
            var result = await controller.GetTransactionSummaryByGroupTypeAndDate(dateRangeAndGroup);

            //// Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result.Result);
            Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        }

        //Get with date range within transactions in data store and valid Transaction Group Type reports a summary based upon the transactions in the data store
        [Fact]
        public async Task Test7_Get_With_Date_Range_Within_Range_And_Valid_Transaction_Group_Type_Reports_Summary_Based_Upon_Transactions_In_Data_Store()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<DataStore>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var dataStore = createDataStore();

            var transactions = SrcTestDataProcessor.ObtainTransactionData(genSrcData, testDataCfg, groupTypesCfg).ToArray();
            dataStore.Transactions.AddRange(transactions);
            dataStore.SaveChanges();

            var controller = new TransactionSummaryController(dataStore);

            //// Act
            var startDate = transactions.First().DateStamp;
            var endDate = transactions.Last().DateStamp;
            var dateRangeAndGroup = new DateFilterAndGroupTypeBodyParams(startDate.ToDateStampString(), endDate.ToDateStampString(), TransactionGroupTypeProcessor.ExpenseTransGroupType);
            var result = await controller.GetTransactionSummaryByGroupTypeAndDate(dateRangeAndGroup);

            //// Assert
            var summaryResult = Assert.IsType<TransactionSummary>(result.Value);
            Assert.NotNull(summaryResult);
            Assert.NotEmpty(summaryResult.Expenditure.Transactions);
            Assert.True(summaryResult.Balance != 0);
            Assert.True(summaryResult.ExpSummaryPct != 0);
        }


    }
}
