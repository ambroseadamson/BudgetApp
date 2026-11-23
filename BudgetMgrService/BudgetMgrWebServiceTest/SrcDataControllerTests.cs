using BudgetMgrServiceLib.Models;
using BudgetMgrWebService.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BudgetMgrWebServiceTest
{
    public class SrcDataControllerTests
    {

        private static readonly IOptions<TestDataCfg> testCfg = Options.Create(TestCfgDataSet.testDataCfg);

        private DataStore createDataStore()
        {
            var options = new DbContextOptionsBuilder<DataStore>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var dataStore = new DataStore(options);
            dataStore.CreditTypesOfInterest.AddRange(TestCfgDataSet.groupTypesCfg.CreditTypesOfInterest);
            dataStore.DebitTypesOfInterest.AddRange(TestCfgDataSet.groupTypesCfg.DebitTypesOfInterest);
            dataStore.TransNames.AddRange(TestCfgDataSet.groupTypesCfg.TransNames);
            dataStore.TransTypes.AddRange(TestCfgDataSet.groupTypesCfg.TransTypes);
            dataStore.TransGroupTypes.AddRange(TestCfgDataSet.groupTypesCfg.TransGroupTypes);
            dataStore.TransGroups.AddRange(TestCfgDataSet.groupTypesCfg.TransGroups);
            dataStore.NameRefs.AddRange(TestCfgDataSet.groupTypesCfg.NameRefs);
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
            var result = await controller.Post(testCfg, new SrcData(linesOfText));

            //// Assert
            var noContentResult = Assert.IsType<NoContentResult>(result.Result);
            Assert.Equal(StatusCodes.Status204NoContent, noContentResult.StatusCode);
        }

        [Fact]
        public async Task Test2_Posting_Corrupt_Data_Reports_An_Bad_Request_Error()
        {
            // Arrange
            var dataStore = createDataStore();
            dataStore.SaveChanges();

            var controller = new SrcDataController(dataStore);

            //// Act
            string[] linesOfText = ["***"];
            var result = await controller.Post(testCfg, new SrcData(linesOfText));

            //// Assert
            var badRequestResult = Assert.IsType<BadRequestResult>(result.Result);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task Test3_Posting_Valid_Data_Reports_A_Success()
        {
            // Arrange
            var dataStore = createDataStore();
            dataStore.SaveChanges();

            var controller = new SrcDataController(dataStore);

            //// Act
            var result = await controller.Post(testCfg, new SrcData(TestCfgDataSet.linesOfText));

            //// Assert
            var okResult = Assert.IsType<OkResult>(result.Result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        }
    }
}
