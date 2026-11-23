using BudgetMgrServiceLib.Generators;
using BudgetMgrServiceLib.Models;
using BudgetMgrServiceLib.Processors;
using BudgetMgrWebService.Controllers;
using BudgetMgrWebService.Processors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using System;
using Xunit;

namespace BudgetMgrWebServiceTest
{

    public class SummaryResultControllerTests
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

        [Fact]
        public async Task Test1_Get_Data_Store_Of_No_Transactions_Reports_Not_Found_Error()
        {
            // Arrange
            var dataStore = createDataStore();
            dataStore.SaveChanges();

            var controller = new SummaryResultController(dataStore);

            //// Act
            var result = await controller.GetSummaryResult();

            //// Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result.Result);
            Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task Test2_Get_Data_Store_Of_Transactions_Reports_The_Transactions_Contained_In_The_Data_Store()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<DataStore>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var dataStore = createDataStore();
            var transactions = SrcTestDataProcessor.ObtainTransactionData(genSrcData, testDataCfg, groupTypesCfg).ToArray();
            dataStore.Transactions.AddRange(transactions);
            dataStore.SaveChanges();

            var controller = new SummaryResultController(dataStore);

            //// Act
            var result = await controller.GetSummaryResult();

            //// Assert
            var summaryResult = Assert.IsType<SummaryResult>(result.Value);
            Assert.NotNull(summaryResult);
            Assert.NotEmpty(summaryResult.SummaryData);
        }

        [Fact]
        public async Task Test3_Get_With_Date_Range_Outside_Of_Range_Reports_Not_Found_Error()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<DataStore>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var dataStore = createDataStore();
            
            var transactions = SrcTestDataProcessor.ObtainTransactionData(genSrcData, testDataCfg, groupTypesCfg).ToArray();
            dataStore.Transactions.AddRange(transactions);
            dataStore.SaveChanges();

            var controller = new SummaryResultController(dataStore);

            //// Act
            var startDate = transactions.Last().DateStamp.AddDays(10);
            var endDate = startDate.AddDays(10);
            var dateRage = new DateFilterBodyParams(startDate.ToDateStampString(), endDate.ToDateStampString());
            var result = await controller.GetSummaryResultByDateRange(dateRage);

            //// Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result.Result);
            Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task Test4_Get_With_Date_Range_Within_Range_Reports_The_Transactions_Within_The_Data_Range()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<DataStore>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var dataStore = createDataStore();

            var transactions = SrcTestDataProcessor.ObtainTransactionData(genSrcData, testDataCfg, groupTypesCfg).ToArray();
            dataStore.Transactions.AddRange(transactions);
            dataStore.SaveChanges();

            var controller = new SummaryResultController(dataStore);

            //// Act
            var startDate = transactions.First().DateStamp;
            var endDate = transactions.Last().DateStamp;
            var dateRage = new DateFilterBodyParams(startDate.ToDateStampString(), endDate.ToDateStampString());
            var result = await controller.GetSummaryResultByDateRange(dateRage);

            //// Assert
            var summaryResult = Assert.IsType<SummaryResult>(result.Value);
            Assert.NotNull(summaryResult);
            Assert.NotEmpty(summaryResult.SummaryData);
        }

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

            var controller = new SummaryResultController(dataStore);

            //// Act
            var startDate = transactions.Last().DateStamp.AddDays(10);
            var endDate = startDate.AddDays(10);
            var dateRangeAndGroup = new DateFilterAndGroupTypeBodyParams(startDate.ToDateStampString(), endDate.ToDateStampString(), "UNKNOWN");
            var result = await controller.GetSummaryResultByDateRangeAndGroupType(dateRangeAndGroup);

            //// Assert
            var badRequestResult = Assert.IsType<BadRequestResult>(result.Result);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        }

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

            var controller = new SummaryResultController(dataStore);

            //// Act
            var startDate = transactions.Last().DateStamp.AddDays(10);
            var endDate = startDate.AddDays(10);
            var dateRangeAndGroup = new DateFilterAndGroupTypeBodyParams(startDate.ToDateStampString(), endDate.ToDateStampString(), TransactionGroupTypeProcessor.ExpenseTransGroupType);
            var result = await controller.GetSummaryResultByDateRangeAndGroupType(dateRangeAndGroup);

            //// Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result.Result);
            Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task Test7_Get_With_Date_Range_Within_Range_Reports_The_Transactions_Within_The_Data_Range()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<DataStore>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var dataStore = createDataStore();

            var transactions = SrcTestDataProcessor.ObtainTransactionData(genSrcData, testDataCfg, groupTypesCfg).ToArray();
            dataStore.Transactions.AddRange(transactions);
            dataStore.SaveChanges();

            var controller = new SummaryResultController(dataStore);

            //// Act
            var startDate = transactions.First().DateStamp;
            var endDate = transactions.Last().DateStamp;
            var dateRangeAndGroup = new DateFilterAndGroupTypeBodyParams(startDate.ToDateStampString(), endDate.ToDateStampString(), TransactionGroupTypeProcessor.ExpenseTransGroupType);
            var result = await controller.GetSummaryResultByDateRangeAndGroupType(dateRangeAndGroup);

            //// Assert
            var summaryResult = Assert.IsType<SummaryResult>(result.Value);
            Assert.NotNull(summaryResult);
            Assert.NotEmpty(summaryResult.SummaryData);
        }
    }
}
