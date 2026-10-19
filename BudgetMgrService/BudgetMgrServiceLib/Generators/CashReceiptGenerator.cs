using System;
using System.Collections.Generic;
using System.Linq;
using BudgetMgrServiceLib.Models;
using BudgetMgrServiceLib.Processors;

namespace BudgetMgrServiceLib.Generators
{
    public static class CashReceiptGenerator
    {
        public static string FileNamePrefix = "cashReceipts-";
        public static string SrcType = "cashReceipts";
        public static SrcData GenCashReceiptData(this TestDataCfg testDataCfg, GroupTypesCfgEx groupTypesCfg, DateTime currentDate, Random rnd)
        {

            var fileNamePrefix = "cashReceipts-";
            var dateFmt = "dd-MMM-yyyy";
            var srcTestDataType = "cashReceipts";

            Func<DateTime> genRndDateStamp = () => rnd.GenRndDateTime(currentDate);
            Func<string> genRndDesc = () => rnd.RandomElement(groupTypesCfg.ToDebitNamesOfInterestWithDefault());
            Func<double> genRndCurrencyVal = () => testDataCfg.ToTransAmount(rnd.ToRandomSrcVal(1, 200), rnd.ToRandomSrcVal(0, 99));

            var applyDateStamp = PropertyLens<CashReceipt, DateTime>.Of(currTrans => currTrans.DateStamp, (currTrans, dateStamp) => currTrans.DateStamp = dateStamp);
            var applyDesc = PropertyLens<CashReceipt, string>.Of(currTrans => currTrans.Desc, (currTrans, desc) => currTrans.Desc = desc);
            var applyTotal = PropertyLens<CashReceipt, double>.Of(currTrans => currTrans.Total, (currTrans, total) => currTrans.Total = total);
            var applyCashPaid = PropertyLens<CashReceipt, double>.Of(currTrans => currTrans.CashPaid, (currTrans, cashPaid) => currTrans.CashPaid = cashPaid);

            var srcValueFns = new List<Func<CashReceipt, CashReceipt>> {
                (currTrans) => currTrans.Map(applyDateStamp,genRndDateStamp()), // DateStamp                
                (currTrans) => currTrans.Map(applyDesc,genRndDesc()),  // Desc
                (currTrans) => currTrans.Map(applyTotal,genRndCurrencyVal()), // Total
                (currTrans) => currTrans.Map(applyCashPaid,genRndCurrencyVal()), // Cash Paid
            };

            Func<int, IEnumerable<CashReceipt>> toCashReceipts = (noOfTrans) => Enumerable.Range(0, noOfTrans).Select(index => srcValueFns.ToTransaction(CashReceiptProcessor.GenDefaultCashReceipt()));

            Func<CashReceipt, DateTime> date = CashReceiptProcessor.TransactionDate;
            Func<CashReceipt, IEnumerable<string>> AsCashReceiptVals = (cashReceiptRow) =>
            {
                return new List<string>(){
                    cashReceiptRow.AsDateStampString(dateFmt, date),
                    cashReceiptRow.Desc,
                    $"{cashReceiptRow.Total}",
                    $"{cashReceiptRow.CashPaid}"
                };
            };

            Func<IEnumerable<CashReceipt>, IEnumerable<string>> toCashReceiptStrings = (cashReceiptRows) => cashReceiptRows.Select(cashReceiptRow => string.Join(",", AsCashReceiptVals(cashReceiptRow)));

            Func<CashReceipt, TestDataCfg, CashReceipt> genSrcMiscReceipt = (receipt, _testDataCfg) =>
            {
                return CashReceipt.Of(receipt.DateStamp,
                                      receipt.Desc,
                                      receipt.Total - testDataCfg.MiscTransactionValue,
                                      receipt.CashPaid - testDataCfg.MiscTransactionValue);
            };

            Func<CashReceipt, string, double, CashReceipt> genMiscReceipt = (receipt, miscReceiptName, miscReceiptValue) =>
            {
                return CashReceipt.Of(receipt.DateStamp,
                                      miscReceiptName,
                                      receipt.Total,
                                      miscReceiptValue);
            };

            // a collection of CashReceipt values, ordering them by date.... 
            Func<CashReceipt, int> dayOfMonth = CashReceiptProcessor.DayOfMonth;
            var _transactions = toCashReceipts(testDataCfg.TransactionRange().Count())
                                .ToOrderedTransactions(dayOfMonth)
                                .AppendMiscTransactions(testDataCfg, genSrcMiscReceipt, genMiscReceipt)
                                .ToArray();

            return SrcData.Of(_transactions.GenSrcFileName(dateFmt, fileNamePrefix, date),
                               srcTestDataType,
                               // a collection of CashReceipt values, rendered as CSV Rows.... 
                               toCashReceiptStrings(_transactions),
                               DateFilterCfg.Of(_transactions.First().DateStamp, _transactions.Last().DateStamp, true));

        }

    }
}