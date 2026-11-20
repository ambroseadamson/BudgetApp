using BudgetMgrServiceLib.Generators;
using BudgetMgrServiceLib.Models;
using BudgetMgrServiceLib.Processors;
using BudgetMgrWebService.Controllers;
using BudgetMgrWebService.Processors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using Xunit;

namespace BudgetMgrWebServiceTest
{
    public class TransactionGroupTypeControllerTests
    {
        public static readonly TestDataCfg testDataCfg = AppCfgProcessor.ToTestDataCfg();
        public static readonly GroupTypesCfgEx groupTypesCfg = AppCfgProcessor.ToGroupTypesCfgEx();
        private DataStore createEmptyDataStore()
        {
            var options = new DbContextOptionsBuilder<DataStore>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new DataStore(options);
        }

        private DataStore createDataStore()
        {
            var dataStore = createEmptyDataStore();
            dataStore.CreditTypesOfInterest.AddRange(groupTypesCfg.CreditTypesOfInterest);
            dataStore.DebitTypesOfInterest.AddRange(groupTypesCfg.DebitTypesOfInterest);
            dataStore.TransNames.AddRange(groupTypesCfg.TransNames);
            dataStore.TransTypes.AddRange(groupTypesCfg.TransTypes);
            dataStore.TransGroupTypes.AddRange(groupTypesCfg.TransGroupTypes);
            dataStore.TransGroups.AddRange(groupTypesCfg.TransGroups);
            dataStore.NameRefs.AddRange(groupTypesCfg.NameRefs);
            return dataStore;
        }

        private IEnumerable<TransactionGroupCfg> genCfgData()
        {
            var dummyDataStore = createDataStore();
            dummyDataStore.SaveChanges();
            return dummyDataStore.ToTransactionGroupCfgs(false);
        }

        //Get data store of no data reports an error(Not Found)
        [Fact]
        public async Task Test1_Get_Data_Store_Of_No_Data_Reports_Not_Found_Error()
        {
            // Arrange
            var dataStore = createEmptyDataStore();
            dataStore.SaveChanges();

            var controller = new TransactionGroupTypeController(dataStore);

            //// Act
            var result = await controller.GetTransactionGroupCfg();

            //// Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result.Result);
            Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        }

        //Get data store of configuration data reports configuration data in the data store
        [Fact]
        public async Task Test2_Get_Data_Store_Of_Config_Data_Reports_Config_Data_Contained_In_The_Data_Store()
        {
            // Arrange
            var dataStore = createDataStore();
            dataStore.SaveChanges();

            var controller = new TransactionGroupTypeController(dataStore);

            //// Act
            var result = await controller.GetTransactionGroupCfg();

            //// Assert
            Assert.NotNull(result.Value);
            Assert.NotEmpty(result.Value);
            Assert.True(result.Value.Count() > 1);
        }

        //Posting No Data reports an error(No Content)
        [Fact]
        public async Task Test3_Posting_No_Data_Reports_No_Content_Error()
        {
            // Arrange
            var dataStore = createEmptyDataStore();
            dataStore.SaveChanges();

            var controller = new TransactionGroupTypeController(dataStore);

            //// Act
            var result = await controller.Post([]);

            //// Assert
            var noContentResult = Assert.IsType<NoContentResult>(result.Result);
            Assert.Equal(StatusCodes.Status204NoContent, noContentResult.StatusCode);
        }

        //Posting corrupt data reports an error(Bad Request)
        [Fact]
        public async Task Test4_Posting_Corrupt_Data_Reports_Bad_Request_Error()
        {
            // Arrange
            var dataStore = createEmptyDataStore();
            dataStore.SaveChanges();

            var controller = new TransactionGroupTypeController(dataStore);

            //// Act
            var result = await controller.Post(null);

            //// Assert
            var badRequestResult = Assert.IsType<BadRequestResult>(result.Result);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        }

        //Posting valid data reports a success(Ok)
        [Fact]
        public async Task Test5_Posting_Valid_Data_Reports_Success_Ok()
        {
            // Arrange
            var dataStore = createEmptyDataStore();
            dataStore.SaveChanges();

            var controller = new TransactionGroupTypeController(dataStore);

            //// Act
            var cfgData = genCfgData().ToArray();
            var result = await controller.Post(cfgData);

            //// Assert
            var okResult = Assert.IsType<OkResult>(result.Result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        }

        //Posting valid data with added transaction names resulting in a datastore with the updated transaction names
        [Fact]
        public async Task Test6_Posting_Valid_Data_With_Added_Transaction_Names_Reports_Datastore_With_Updated_Transaction_Names()
        {

        }


        //Posting valid data with modified transaction names resulting in a datastore with the modified transaction names
        [Fact]
        public async Task Test7_Posting_Valid_Data_With_Modified_Transaction_Names_Reports_Datastore_With_Modified_Transaction_Names()
        {

        }

        //Posting valid data with removed transaction names resulting in a datastore with the relevant transaction names removed
        [Fact]
        public async Task Test8_Posting_Valid_Data_With_Removed_Transaction_Names_Reports_Datastore_With_Relevant_Transaction_Names_Removed()
        {

        }

        //Posting valid data with added transaction groups resulting in a datastore with the updated transaction groups
        [Fact]
        public async Task Test9_Posting_Valid_Data_With_Added_Transaction_Groups_Reports_Datastore_With_Updated_Transaction_Groups()
        {

        }

        //Posting valid data with removed transaction groups resulting in a datastore with the relevant transaction names groups
        [Fact]
        public async Task Test10_Posting_Valid_Data_With_Removed_Transaction_Groups_Reports_Datastore_With_Relevant_Transaction_Groups_Removed()
        {

        }

    }
}
