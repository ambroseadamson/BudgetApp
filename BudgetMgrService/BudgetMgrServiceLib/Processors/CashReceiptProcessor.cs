using System;
using BudgetMgrServiceLib.Models;

namespace BudgetMgrServiceLib.Processors
{
    public static class CashReceiptProcessor
    {
        public static int DayOfMonth(this CashReceipt cashReceipt) => cashReceipt.DateStamp.Day;
        public static DateTime TransactionDate(this CashReceipt transaction) => transaction.DateStamp;

        public static DateTime GenRndDateTime(this Random rnd, DateTime currentDate) => new DateTime(currentDate.Year, currentDate.Month, rnd.ToRandomSrcVal(2, 28));
        public static CashReceipt GenDefaultCashReceipt() => CashReceipt.Of(DateTime.Now, "N/A", 0.0, 0.0);
    }
}