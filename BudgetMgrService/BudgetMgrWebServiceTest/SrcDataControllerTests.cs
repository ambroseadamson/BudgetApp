using BudgetMgrServiceLib.Generators;
using BudgetMgrServiceLib.Models;
using BudgetMgrServiceLib.Processors;
using BudgetMgrWebService.Controllers;
using BudgetMgrWebService.Processors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
namespace BudgetMgrWebServiceTest
{
    public class SrcDataControllerTests
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
        public async Task Test1_Posting_No_Data_Reports_An_No_Content_Error()
        {
            // Arrange
            var dataStore = createDataStore();
            dataStore.SaveChanges();

            var controller = new SrcDataController(dataStore);

            //// Act
            string[] linesOfText = [];
            var testCfg = new Mock<IOptions<TestDataCfg>>();
            testCfg.Setup(x => x.Value).Returns(testDataCfg);

            var result = await controller.Post(testCfg.Object, new SrcData(linesOfText));

            //// Assert
            var noContentResult = Assert.IsType<NoContentResult>(result.Result);
            Assert.Equal(StatusCodes.Status204NoContent, noContentResult.StatusCode);
        }

        [Fact]
        public async Task Test2_Posting_Corrupt_Data_Reports_An_Bad_Request_Error()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<DataStore>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var dataStore = createDataStore();
            dataStore.SaveChanges();

            var controller = new SrcDataController(dataStore);

            //// Act
            string[] linesOfText = ["***"];
            var testCfg = new Mock<IOptions<TestDataCfg>>();
            testCfg.Setup(x => x.Value).Returns(testDataCfg);

            var result = await controller.Post(testCfg.Object, new SrcData(linesOfText));

            //// Assert
            var badRequestResult = Assert.IsType<BadRequestResult>(result.Result);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);

        }

        [Fact]
        public async Task Test3_Posting_Valid_Data_Reports_A_Success()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<DataStore>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var dataStore = createDataStore();
            dataStore.SaveChanges();

            var controller = new SrcDataController(dataStore);

            //// Act
            string[] linesOfText = SrcTestDataProcessor.ObtainCsvData(genSrcData, testDataCfg, groupTypesCfg).ToArray();

            var testCfg = new Mock<IOptions<TestDataCfg>>();
            testCfg.Setup(x => x.Value).Returns(testDataCfg);

            var result = await controller.Post(testCfg.Object, new SrcData(linesOfText));

            //// Assert
            var okResult = Assert.IsType<OkResult>(result.Result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        }
    }
}
