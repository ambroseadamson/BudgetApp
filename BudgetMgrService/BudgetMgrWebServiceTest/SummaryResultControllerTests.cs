using System;
using Xunit;

namespace BudgetMgrWebServiceTest
{

    public class SummaryResultControllerTests 
    {
        [Fact]
        public void Test1_Get_Data_Store_Of_No_Transactions_Reports_Not_Found_Error()
        {

        }

        [Fact]
        public void Test2_Get_Data_Store_Of_Transactions_Reports_The_Transactions_Contained_In_The_Data_Store()
        {

        }

        [Fact]
        public void Test3_Get_With_Date_Range_Outside_Of_Range_Reports_Not_Found_Error()
        {

        }

        [Fact]
        public void Test4_Get_With_Date_Range_Within_Range_Reports_The_Transactions_Within_The_Data_Range()
        {

        }

        [Fact]
        public void Test5_Get_With_Date_Range_Outside_Of_Range_And_Invalid_Transaction_Group_Type_Reports_Bad_Request_Error()
        {

        }

        [Fact]
        public void Test6_Get_With_Date_Range_Outside_Of_Range_And_Valid_Transaction_Group_Type_Reports_Not_Found_Error()
        {

        }

        [Fact]
        public void Test7_Get_With_Date_Range_Within_Range_Reports_The_Transactions_Within_The_Data_Range()
        {

        }
    }
}
