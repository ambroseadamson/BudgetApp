using BudgetApp.Controllers.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BudgetApp.Controllers.Model
{
    public class SrcTestDataResults
    {
        public SrcTestDataResults() {}
        
        public string ResultName {set; get;}
        public IEnumerable<string> Results {set; get;}
    }
    
    public class SrcTestDataExt
    {
        public static int ToRandomSrcVal(this Random rnd, int min, int max) => AppCfgExt.ToRandomSrcVal(rnd, min, max);


        public static U ToTransaction<T,U>(this IEnumerable<Func<T,U>> fns) => fns.Aggregate(default<U>, (fn,currVal) => currVal = fn(currVal));

        public static int ToDayOfMonth(this Transaction transaction) => transaction.DateStamp.Day;

        // toOrderedTransactions :: ([int] -> ([Transaction]|[CashReceipt])) -> String -> ([Transaction]|[CashReceipt]) -> ([Transaction]|[CashReceipt])
        public static IEnumerable<T> ToOrderedTransactions(this IEnumerable<T> transactions) => transactions.OrderBy(transaction => ToDayOfMonth(transaction));

        public static IEnumerable<int> TransactionRange(AppCfg appCfg) => Enumerable.Range(0, appCfg.numberOfTransactions);

        public static string AsDateStampString(this Transaction transaction, string dateFormat) => transaction.DateStamp.ToString(dateFormat);

        public static string GenSrcFileName(this IEnumerable<Transaction> transData, string dateFmt, string fileNamePrefix)
        {
            return string.Format("{0}{1)-to-{2}.csv", 
                                fileNamePrefix, 
                                AsDateStampString(transData.First(), dateFmt), 
                                AsDateStampString(transData.Last(), dateFmt));
        }

        // genTestStatementData :: AppCfg -> Date -> (String = Filename|[String = CSV Row])
        // Handler for the REST request "/genTestStatementData" 
        public static SrcTestDataResults GenTestStatementData(AppCfg appCfg, DateTime currentDate) 
        {
            var fileNamePrefix = "Statement Download-";
            var parseDateFmt = "DD MMM YYYY";
            var fileNameDateFmt = "DD-MMM-YYYY";

            // fromTD :: AppCfg -> Date -> int -> int -> double -> Transaction
            fromTD = R.curry((appCfg, dateStamp, transTypeIndex, transNameIndex, amount, useDebit) => {
                const transNamesOfInterest = (useDebit) ? appCfg.debitNamesOfInterest : appCfg.creditNamesOfInterest;
                return TD.ofTransaction(dateStamp, AppCfg.toTransType(appCfg.transTypes, transTypeIndex), AppCfg.toTransName(transNamesOfInterest, transNameIndex), amount);
            });

            let srcValueFns = [
                (currDate) => new Transaction() { DateStamp= AppCfg.ToDateStamp(currentDate, AppCfg.ToRandomSrcVal(2, 28))} , // DateStamp
                () => exports.toRandomSrcVal(1, appCfg.transTypes.length - 1), // TransType
                () => exports.toRandomSrcVal(0, appCfg.debitNamesOfInterest.length - 1), // DebitName
                () => AppCfg.toTransAmount(exports.toRandomSrcVal(1, 200), exports.toRandomSrcVal(0, 99)), // Amount in Pounds and Pence
                () => true
            ];

        // toCreditTransaction :: [Transaction] -> [Transaction, count = 1]
        let toCreditTransaction = (transactions) => [fromTD(appCfg, R.prop('dateStamp', R.head(transactions)), 0, 0, sumAmounts(transactions), false)];

        // toTransactions :: [int] -> [Transaction]
        let toTransactions = R.map(x => exports.toTransaction(fromTD, srcValueFns));

        // toTransactionValString :: a -> String 
        let toTransactionValString = (val) => '\"' + val + '\"';

        // isTransType :: [String] -> Transaction -> Boolean
        let isTransType = R.curry((transTypesOfInterest, transaction) => R.any((_transType) => _transType === transaction.transType, transTypesOfInterest));

        // asCurrencyValString :: (Transaction -> Boolean) -> Transaction -> [String]
        let asCurrencyValString = (amountType, transaction) => (amountType(transaction)) ? '�' + transaction.amount.toString() : '';

        // asTransactionVals :: Transaction -> [String]
        let asTransactionVals = R.map((transaction) => [
            exports.asDateStampString(datePropName, parseDateFmt, transaction),
            transaction.transType,
            transaction.description,
            asCurrencyValString(isTransType(appCfg.debitTypesOfInterest), transaction),
            asCurrencyValString(isTransType(appCfg.creditTypesOfInterest), transaction),
            '�0.00'
        ]);

        // toTransactionString :: [String] -> String
        let toTransactionString = R.map(R.compose(R.join(','), R.map(toTransactionValString)));

        // toTransactionStrings :: [Transaction] -> [String]
        let toTransactionStrings = R.compose(toTransactionString, asTransactionVals);

        // sumAmounts :: [Transaction] -> number
        let sumAmounts = R.compose(R.sum, R.map(R.prop('amount')));

        // toTransactionSet :: [Transaction] -> [Transaction]
        let toTransactionSet = (transactions) => R.concat(toCreditTransaction(transactions), R.drop(1, transactions));

        // a collection of Transaction values, ordering them by date.... 
        let _transactions = exports.toOrderedTransactions(toTransactions, datePropName, exports.transactionRange(appCfg));

        let transactionHeader = [
            '\"Account Name:\",\"FlexAccount ****97018\"',
            '\"Account Balance:\",\"£83.91\"',
            '\"Available Balance: \",\"�30.31\"',
            '',
            '\"Date\",\"Transaction type\",\"Description\",\"Paid out\",\"Paid in\",\"Balance\"'
        ];
            return {
                statementSrcFileName: exports.genSrcFileName(datePropName, fileNameDateFmt, fileNamePrefix, _transactions),

                // a collection of Transaction values, prepended with a header and rendered as CSV Rows.... 
                transactions: R.concat(transactionHeader, R.compose(toTransactionStrings, toTransactionSet) (_transactions))
            };
        });

        // genTestCashReceiptData :: AppCfg -> Date -> (String = Filename|[String = CSV Row])
        // Handler for the REST request "/genTestCashReceiptData"
        exports.genTestCashReceiptData = R.curry((appCfg, currentDate) => {

            const datePropName = 'date';
        const fileNamePrefix = 'cashReceipts-';
        const dateFmt = 'DD-MMM-YYYY';

        // fromCR :: AppCfg -> Date -> int -> double -> double -> CashReceipt
        fromCR = R.curry((appCfg, dateStamp, transNameIndex, amount, cashPaid) => {
                const transNamesOfInterest = appCfg.debitNamesOfInterest;
                return TD.ofCashReceipt(dateStamp, AppCfg.toTransName(transNamesOfInterest, transNameIndex), amount, cashPaid);
            });

            let srcValueFns = [
                () => appCfg,
                () => AppCfg.toJSONDate(currentDate, exports.toRandomSrcVal(2, 28)), // DateStamp
                () => exports.toRandomSrcVal(0, appCfg.debitNamesOfInterest.length - 1), // DebitName
                () => AppCfg.toTransAmount(exports.toRandomSrcVal(1, 200), exports.toRandomSrcVal(0, 99)), // Amount in Pounds and Pence
                () => AppCfg.toTransAmount(exports.toRandomSrcVal(1, 200), exports.toRandomSrcVal(0, 99)), // Cash Paid in Pounds and Pence
            ];

        // toCashReceipts :: [int] -> [CashReceipt]
        let toCashReceipts = R.map(x => exports.toTransaction(fromCR, srcValueFns));

        // asCashReceiptVals :: CashReceipt -> [String]
        let asCashReceiptVals = R.map((cashReceiptRow) => [
            exports.asDateStampString(datePropName, dateFmt, cashReceiptRow),
            cashReceiptRow.desc,
            cashReceiptRow.total.toString(),
            cashReceiptRow.cashPaid.toString()
        ]);

        // toCashReceiptStrings :: [CashReceipt] -> [String]
        let toCashReceiptStrings = R.compose(R.map(R.join(',')), asCashReceiptVals);

        // a collection of CashReceipt values, ordering them by date.... 
        let _transactions = exports.toOrderedTransactions(toCashReceipts, datePropName, exports.transactionRange(appCfg));

            return {
                statementSrcFileName: exports.genSrcFileName(datePropName, dateFmt, fileNamePrefix, _transactions),

                // a collection of CashReceipt values, rendered as CSV Rows.... 
                transactions: toCashReceiptStrings(_transactions)
            };
        });

    }
}
