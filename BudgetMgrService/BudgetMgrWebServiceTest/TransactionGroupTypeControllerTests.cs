using BudgetMgrServiceLib.Models;
using BudgetMgrServiceLib.Processors;
using BudgetMgrWebService.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BudgetMgrWebServiceTest
{
    public sealed class TransactionGroupCfgComparer : IEqualityComparer<TransactionGroupCfg>
    {
        public bool Equals(TransactionGroupCfg? x, TransactionGroupCfg? y)
        {
            if ((x == null) || (y == null))
            {
                return false;
            }

            return ((x.GroupName == y.GroupName) && (x.GroupType == y.GroupType) && (x.Names.SequenceEqual(y.Names)));
        }
        public int GetHashCode(TransactionGroupCfg obj) => HashCode.Combine(obj.GroupName, obj.GroupType, obj.Names);
    }

    public class TransactionGroupTypeControllerTests
    {
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
            dataStore.CreditTypesOfInterest.AddRange(TestCfgDataSet.groupTypesCfg.CreditTypesOfInterest);
            dataStore.DebitTypesOfInterest.AddRange(TestCfgDataSet.groupTypesCfg.DebitTypesOfInterest);
            dataStore.TransNames.AddRange(TestCfgDataSet.groupTypesCfg.TransNames);
            dataStore.TransTypes.AddRange(TestCfgDataSet.groupTypesCfg.TransTypes);
            dataStore.TransGroupTypes.AddRange(TestCfgDataSet.groupTypesCfg.TransGroupTypes);
            dataStore.TransGroups.AddRange(TestCfgDataSet.groupTypesCfg.TransGroups);
            dataStore.NameRefs.AddRange(TestCfgDataSet.groupTypesCfg.NameRefs);
            return dataStore;
        }

        private IEnumerable<TransactionGroupCfg> obtainFixedCfgData() => TestCfgDataSet.fixedCfgData.Select(group => group with { Names = group.Names.ToArray() });

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
            var cfgData = obtainFixedCfgData().ToArray();
            var result = await controller.Post(cfgData);

            //// Assert
            var okResult = Assert.IsType<OkResult>(result.Result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        }

        //Posting valid data with added transaction names resulting in a datastore with the updated transaction names
        [Fact]
        public async Task Test6_Posting_Valid_Data_With_Added_Transaction_Names_Reports_Datastore_With_Updated_Transaction_Names()
        {
            // Arrange
            var dataStore = createEmptyDataStore();
            dataStore.SaveChanges();

            var controller = new TransactionGroupTypeController(dataStore);

            //// Act
            var cfgData = obtainFixedCfgData().ToArray();


            var additionalNames = Enumerable.Range(1, 3).Select(Id => $"ADDITIONAL LOAN {Id}");
            cfgData = cfgData.Select(group =>
            {
                if (group.GroupName == "loans")
                {
                    group = group with { Names = group.Names.Concat(additionalNames).ToArray() };
                }
                return group;
            }).ToArray();

            var result = await controller.Post(cfgData);

            //// Assert
            var okResult = Assert.IsType<OkResult>(result.Result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);

            var groupCfgResult = await controller.GetTransactionGroupCfg();

            Assert.NotNull(groupCfgResult.Value);
            Assert.NotEmpty(groupCfgResult.Value);
            Assert.True(groupCfgResult.Value.Count() > 1);

            var loansGroup = groupCfgResult.Value.First(group => (group.GroupName == "loans"));
            Assert.Contains(loansGroup.Names, name => additionalNames.Contains(name));
        }


        //Posting valid data with modified transaction names resulting in a datastore with the modified transaction names
        [Fact]
        public async Task Test7_Posting_Valid_Data_With_Modified_Transaction_Names_Reports_Datastore_With_Modified_Transaction_Names()
        {
            // Arrange
            var dataStore = createEmptyDataStore();
            dataStore.SaveChanges();

            var controller = new TransactionGroupTypeController(dataStore);

            //// Act
            var cfgData = obtainFixedCfgData().ToArray();


            var modifiedlNames = new string[] { };
            cfgData = cfgData.Select(group =>
            {
                if (group.GroupName == "loans")
                {
                    var index = 1;
                    modifiedlNames = group.Names.Select(name =>
                    {
                        if (index <= 3)
                        {
                            name = $"{name}{index}";
                        }
                        index++;
                        return name;
                    }).ToArray();
                    group = group with { Names = modifiedlNames };
                }
                return group;
            }).ToArray();

            var result = await controller.Post(cfgData);

            //// Assert
            var okResult = Assert.IsType<OkResult>(result.Result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);

            var groupCfgResult = await controller.GetTransactionGroupCfg();

            Assert.NotNull(groupCfgResult.Value);
            Assert.NotEmpty(groupCfgResult.Value);
            Assert.True(groupCfgResult.Value.Count() > 1);

            var loansGroup = groupCfgResult.Value.First(group => (group.GroupName == "loans"));
            Assert.Equal(modifiedlNames.Order(), loansGroup.Names.Order());
        }

        //Posting valid data with removed transaction names resulting in a datastore with the relevant transaction names removed
        [Fact]
        public async Task Test8_Posting_Valid_Data_With_Removed_Transaction_Names_Reports_Datastore_With_Relevant_Transaction_Names_Removed()
        {
            // Arrange
            var dataStore = createEmptyDataStore();
            dataStore.SaveChanges();

            var controller = new TransactionGroupTypeController(dataStore);

            //// Act
            var cfgData = obtainFixedCfgData().ToArray();


            var removedNames = new string[] { };
            cfgData = cfgData.Select(group =>
            {
                if (group.GroupName == "loans")
                {
                    removedNames = group.Names.Take(3).ToArray();
                    group = group with { Names = group.Names.Skip(3).ToArray() };
                }
                return group;
            }).ToArray();

            var result = await controller.Post(cfgData);

            //// Assert
            var okResult = Assert.IsType<OkResult>(result.Result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);

            var groupCfgResult = await controller.GetTransactionGroupCfg();

            Assert.NotNull(groupCfgResult.Value);
            Assert.NotEmpty(groupCfgResult.Value);
            Assert.True(groupCfgResult.Value.Count() > 1);

            var loansGroup = groupCfgResult.Value.First(group => (group.GroupName == "loans"));
            Assert.DoesNotContain(loansGroup.Names, removedNames.AsCollection());
        }

        //Posting valid data with added transaction groups resulting in a datastore with the updated transaction groups
        [Fact]
        public async Task Test9_Posting_Valid_Data_With_Added_Transaction_Groups_Reports_Datastore_With_Updated_Transaction_Groups()
        {
            // Arrange
            var dataStore = createEmptyDataStore();
            dataStore.SaveChanges();

            var controller = new TransactionGroupTypeController(dataStore);

            //// Act
            var cfgData = obtainFixedCfgData().ToArray();

            var addedGroup = new [] {
                new TransactionGroupCfg(TransactionGroupTypeProcessor.DefaultId,
                                        "AddedGroupName",
                                        TransactionGroupTypeProcessor.ExpenseTransGroupType,
                                        [
                                            "EXPENSE ITEM 1",
                                            "EXPENSE ITEM 2"
                                            ])
            };
            cfgData = cfgData.Concat(addedGroup).ToArray();

            var result = await controller.Post(cfgData);

            //// Assert
            var okResult = Assert.IsType<OkResult>(result.Result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);

            var groupCfgResult = await controller.GetTransactionGroupCfg();

            Assert.NotNull(groupCfgResult.Value);
            Assert.NotEmpty(groupCfgResult.Value);
            Assert.True(groupCfgResult.Value.Count() > 1);

            var addedGroupResult = groupCfgResult.Value.First(group => group.GroupName == "AddedGroupName");
            Assert.Equal(addedGroup.First().GroupType, addedGroupResult.GroupType);
            Assert.Equal(addedGroup.First().Names.Order(), addedGroupResult.Names.Order());
        }

        //Posting valid data with removed transaction groups resulting in a datastore with the relevant transaction names groups
        [Fact]
        public async Task Test10_Posting_Valid_Data_With_Removed_Transaction_Groups_Reports_Datastore_With_Relevant_Transaction_Groups_Removed()
        {
            // Arrange
            var dataStore = createEmptyDataStore();
            dataStore.SaveChanges();

            var controller = new TransactionGroupTypeController(dataStore);

            //// Act
            var cfgData = obtainFixedCfgData().ToArray();

            var groupToRemove = cfgData.First(group => group.GroupName == "loans");
            groupToRemove = groupToRemove with { Id = TransactionGroupTypeProcessor.RemovedGroupId };
            cfgData = cfgData.Select(group => (group.GroupName != "loans") ? group : groupToRemove).ToArray();

            var result = await controller.Post(cfgData);

            //// Assert
            var okResult = Assert.IsType<OkResult>(result.Result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);

            var groupCfgResult = await controller.GetTransactionGroupCfg();

            Assert.NotNull(groupCfgResult.Value);
            Assert.NotEmpty(groupCfgResult.Value);
            Assert.True(groupCfgResult.Value.Count() > 1);

            var comparer = new TransactionGroupCfgComparer();
            Assert.DoesNotContain(groupToRemove, groupCfgResult.Value, comparer);

            // confirm that there are no "orphaned" transaction names
            var miscGroups = groupCfgResult.Value.First(group => group.GroupName == TransactionGroupTypeProcessor.MiscTransGroupType);
            Assert.Empty(miscGroups.Names);
        }
    }
}
