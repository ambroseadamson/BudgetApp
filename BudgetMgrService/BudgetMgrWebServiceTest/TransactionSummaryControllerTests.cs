using System;
using System.Transactions;
using Xunit;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BudgetMgrWebServiceTest
{
    public class TransactionSummaryControllerTests
    {
        //Get data store of no transactions reports an error(Not Found)
        [Fact]
        public void Test1_Get_Data_Store_Of_No_Transactions_Reports_Not_Found_Error()
        {

        }

        //Get data store of transactions reports a summary based upon the transactions in the data store
        [Fact]
        public void Test2_Get_Data_Store_Of_Transactions_Reports_Summary_Based_Upon_Transactions_In_Data_Store()
        {

        }

        //Get with date range outside of transactions in data store reports an error(Not Found)
        [Fact]
        public void Test3_Get_Date_Range_Outside_Of_Transactions_Reports_Not_Found_Error()
        {

        }

        //Get with date range within transactions reports a summary based upon the transactions in the data store
        [Fact]
        public void Test4_Get_Date_Range_Within_Transactions_Reports_Summary_Based_Upon_Transactions_In_Data_Store()
        {

        }

        //Get with date range outside of transactions in data store and invalid Transaction Group Type reports an error(Bad Request)
        [Fact]
        public void Test5_Get_Date_Range_Outside_Of_Range_And_Invalid_Transaction_Group_Type_Reports_Bad_Request_Error()
        {

        }

        //Get with date range outside of transactions in data store and valid Transaction Group Type reports an error(Not Found)
        [Fact]
        public void Test6_Get_Date_Range_Outside_Of_Range_And_Valid_Transaction_Group_Type_Reports_Not_Found_Error()
        {

        }

        //Get with date range within transactions in data store and valid Transaction Group Type reports a summary based upon the transactions in the data store
        [Fact]
        public void Test7_Get_Date_Range_Within_Range_And_Valid_Transaction_Group_Type_Reports_Summary_Based_Upon_Transactions_In_Data_Store()
        {

        }


    }
}
