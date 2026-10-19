using System;
using System.Collections.Generic;
using System.Linq;
using BudgetMgrServiceLib.Models;
using BudgetMgrServiceLib.Processors;

namespace BudgetMgrServiceLib.Generators
{
    public static class StatementGenerator
    {
        public static string FileNamePrefix = "Statement Download-";
        public static string SrcType = "Statement Download";

        public static SrcData GenStatementData(this TestDataCfg testDataCfg, GroupTypesCfgEx groupTypesCfg, DateTime currentDate, Random rnd)
        {
            var fileNamePrefix = "Statement Download-";
            var parseDateFmt = "dd MMM yyyy";
            var fileNameDateFmt = "dd-MMM-yyyy";
            var srcTestDataType = "Statement Download";

            var debitTypesOfInterest = groupTypesCfg.ToDebitTypesOfInterestEx().Where(debitType => debitType != "CASH RECEIPT");


            Func<DateTime> genRndDateStamp = () => new DateTime(currentDate.Year, currentDate.Month, rnd.ToRandomSrcVal(2, 28));
            Func<string> genRndTransType = () => rnd.RandomElement(debitTypesOfInterest);
            Func<string> genRndDebitName = () => rnd.RandomElement(groupTypesCfg.ToDebitNamesOfInterestWithDefault());
            Func<double> genRndCurrencyVal = () => testDataCfg.ToTransAmount(rnd.ToRandomSrcVal(1, 200), rnd.ToRandomSrcVal(0, 99));

            var applyDateStamp = PropertyLens<Transaction, DateTime>.Of(currTrans => currTrans.DateStamp, (currTrans, dateStamp) => currTrans.DateStamp = dateStamp);
            var applyTransType = PropertyLens<Transaction, string>.Of(currTrans => currTrans.TransType, (currTrans, transType) => currTrans.TransType = transType);
            var applyDebitName = PropertyLens<Transaction, string>.Of(currTrans => currTrans.Description, (currTrans, debitName) => currTrans.Description = debitName);
            var applyAmount = PropertyLens<Transaction, double>.Of(currTrans => currTrans.Amount, (currTrans, amount) => currTrans.Amount = amount);

            var srcValueFns = new List<Func<Transaction, Transaction>> {
                (currTrans) => currTrans.Map(applyDateStamp,genRndDateStamp()),              // DateStamp                
                (currTrans) => currTrans.Map(applyTransType,genRndTransType()),            // TransType
                (currTrans) => currTrans.Map(applyDebitName,genRndDebitName()),     // DebitName
                (currTrans) => currTrans.Map(applyAmount,genRndCurrencyVal())        // Amount in Pounds and Pence
            };

            Func<IEnumerable<Transaction>, IEnumerable<Transaction>> toCreditTransaction = (transactions) =>
            {
                var transaction = transactions.First();
                return new List<Transaction>() {
                    Transaction.Of(0, transaction.DateStamp, groupTypesCfg.ToTransTypesEx().First(),
                                    groupTypesCfg.ToCreditNamesOfInterestWithDefault().First(),
                                    transactions.Select(trans => trans.Amount).Sum())
                };
            };

            Func<int, IEnumerable<Transaction>> toTransactions = (noOfTrans) => Enumerable.Range(0, noOfTrans).Select(index => srcValueFns.ToTransaction(Transaction.Of(0, DateTime.Now, "", "", 0.0)));

            string ToTransactionValString<T>(T val) => string.Format("\"{0}\"", val);

            Func<IEnumerable<string>, Transaction, bool> isTransType = (transTypesOfInterest, transaction) => transTypesOfInterest.Any(_transType => _transType == transaction.TransType);

            Func<IEnumerable<string>, Func<IEnumerable<string>, Transaction, bool>, Transaction, string> asCurrencyValString = (transTypesOfInterest, amountType, transaction) => (amountType(transTypesOfInterest, transaction) == true) ? $"�{transaction.Amount}" : "";

            Func<Transaction, DateTime> date = TransactionProcessor.TransactionDate;
            Func<Transaction, IEnumerable<string>> asTransactionVals = (transaction) =>
            {
                return new List<string>(){
                    transaction.AsDateStampString(parseDateFmt, date),
                    transaction.TransType,
                    transaction.Description,
                    asCurrencyValString(groupTypesCfg.ToDebitTypesOfInterestEx(), isTransType, transaction),
                    asCurrencyValString(groupTypesCfg.ToCreditTypesOfInterestEx(), isTransType, transaction),
                    "�0.00"
                };
            };

            Func<IEnumerable<string>, string> toTransactionString = (vals) => string.Join(",", vals.Select(val => ToTransactionValString(val)));

            Func<IEnumerable<Transaction>, IEnumerable<string>> ToTransactionStrings = (transactions) => transactions.Select(asTransactionVals).Select(toTransactionString);

            Func<IEnumerable<Transaction>, double> SumAmounts = (transactions) => transactions.Sum(transaction => transaction.Amount);

            Func<IEnumerable<Transaction>, IEnumerable<Transaction>> ToTransactionSet = (transactions) => toCreditTransaction(transactions).Concat(transactions.Skip(1));

            // a collection of Transaction values, ordering them by date....
            Func<Transaction, int> dayOfMonth = TransactionProcessor.DayOfMonth;

            Func<Transaction, TestDataCfg, Transaction> genSrcMiscTransaction = (transaction, _testDataCfg) =>
            {
                return Transaction.Of(0,
                                      transaction.DateStamp,
                                      transaction.TransType,
                                      transaction.Description,
                                      transaction.Amount - testDataCfg.MiscTransactionValue);
            };

            Func<Transaction, string, double, Transaction> genMiscTransaction = (transaction, miscTransactionName, miscTransactionValue) =>
            {
                return Transaction.Of(0,
                                      transaction.DateStamp,
                                      transaction.TransType,
                                      miscTransactionName,
                                      miscTransactionValue);

            };

            var _transactions = toTransactions(testDataCfg.TransactionRange().Count())
                                .ToOrderedTransactions(dayOfMonth)
                                .AppendMiscTransactions(testDataCfg, genSrcMiscTransaction, genMiscTransaction)
                                .ToArray();

            string[] transactionHeader = {
                "\"Account Name:\",\"FlexAccount ****97018\"",
                "\"Account Balance:\",\"£83.91\"",
                "\"Available Balance: \",\"�30.31\"",
                "",
                "\"Date\",\"Transaction type\",\"Description\",\"Paid out\",\"Paid in\",\"Balance\""
            };

            return SrcData.Of(_transactions.GenSrcFileName(fileNameDateFmt, fileNamePrefix, date),
                               srcTestDataType,
                               // a collection of Transaction values, prepended with a header and rendered as CSV Rows.... 
                               transactionHeader.Concat(ToTransactionStrings(ToTransactionSet(_transactions))),
                               DateFilterCfg.Of(_transactions.First().DateStamp, _transactions.Last().DateStamp, true));
        }


    }
}