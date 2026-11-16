using System;
using Xunit;

namespace BudgetMgrWebServiceTest
{
    public class TransactionGroupTypeControllerTests
    {
        //Get data store of no data reports an error(Not Found)
        [Fact]
        public void Test1_Get_Data_Store_Of_No_Data_Reports_Not_Found_Error()
        {

        }

        //Get data store of configuration data reports configuration data in the data store
        [Fact]
        public void Test2_Get_Data_Store_Of_Config_Data_Reports_Config_Data_Contained_In_The_Data_Store()
        {

        }

        //Posting No Data reports an error(No Content)
        [Fact]
        public void Test3_Posting_No_Data_Reports_Not_Content_Error()
        {

        }

        //Posting corrupt data reports an error(Bad Request)
        [Fact]
        public void Test4_Posting_Corrupt_Data_Reports_Bad_Request_Error()
        {

        }

        //Posting valid data reports a success(Ok)
        [Fact]
        public void Test5_Posting_Valid_Data_Reports_Success_Ok()
        {

        }

        //Posting valid data with added transaction names resulting in a datastore with the updated transaction names
        [Fact]
        public void Test6_Posting_Valid_Data_With_Added_Transaction_Names_Reports_Datastore_With_Updated_Transaction_Names()
        {

        }


        //Posting valid data with modified transaction names resulting in a datastore with the modified transaction names
        [Fact]
        public void Test7_Posting_Valid_Data_With_Modified_Transaction_Names_Reports_Datastore_With_Modified_Transaction_Names()
        {

        }

        //Posting valid data with removed transaction names resulting in a datastore with the relevant transaction names removed
        [Fact]
        public void Test8_Posting_Valid_Data_With_Removed_Transaction_Names_Reports_Datastore_With_Relevant_Transaction_Names_Removed()
        {

        }

        //Posting valid data with added transaction groups resulting in a datastore with the updated transaction groups
        [Fact]
        public void Test9_Posting_Valid_Data_With_Added_Transaction_Groups_Reports_Datastore_With_Updated_Transaction_Groups()
        {

        }

        //Posting valid data with removed transaction groups resulting in a datastore with the relevant transaction names groups
        [Fact]
        public void Test10_Posting_Valid_Data_With_Removed_Transaction_Groups_Reports_Datastore_With_Relevant_Transaction_Groups_Removed()
        {

        }

    }
}
