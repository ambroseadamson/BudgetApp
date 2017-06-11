using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using BudgetApp.Controllers.Config;

namespace BudgetApp.Controllers.Model
{
    public class Transaction
    {
        public Transaction()
        {
        }


        public DateTime dateStamp { set; get; }
        public string transType { set; get; }
        public string description { set; get; }
        public double amount { set; get; }
    }


    public class CashReceipt
    {
        public CashReceipt()
        {
        }
        public DateTime date { get; set; }
        public string desc { get; set; }
        public double total { get; set; }
        public double cashPaid { get; set; }

    }

    public class TransactionResults
    {
        public TransactionResults()
        {
        }

        public string ResultName { get; set; }
        public IEnumerable<Transaction> Results { get; set; }
    }

    public static class TransactionExt
    {
        // genTransactionData :: AppCfg -> [String] -> [String] -> JSON
        // Handler for the REST request "/genTransactionData" 
        public static TransactionResults GenTransactionData(this IEnumerable<string> transactionSrcData, AppCfg appCfg, string srcType)
        {

            Func<int, IEnumerable<string>, string> extractVal = (n, entries) => entries.Skip(n).First();

            // toDateStamp :: Boolean -> String -> Date
            //            Func<bool, string, DateTime> toDateStamp = (fromCashReceipt, val) => DateTime.Parse(val, (fromCashReceipt == true) ? "DD-MMM-YYYY" : "DD MMM YYYY");
            Func<string, DateTime> toDateStamp = (val) => DateTime.Parse(val);

            Func<IEnumerable<string>, CashReceipt> toCashReceipt = (entry) =>
            {
                return new CashReceipt()
                {
                    date = toDateStamp(entry.First()),
                    desc = extractVal(1, entry),
                    total = Double.Parse(extractVal(2, entry)),
                    cashPaid = Double.Parse(extractVal(3, entry))
                };
            };

            Func<CashReceipt, Transaction> toTransactionFromCR = (cashReceipt) =>
            {
                return new Transaction()
                {
                    dateStamp = cashReceipt.date,
                    transType = "CASH RECEIPT",
                    description = cashReceipt.desc,
                    amount = cashReceipt.total
                };
            };

            Func<IEnumerable<string>, IEnumerable<Transaction>> genCashReceipts = (entries) =>
            {
                return entries.Select(entry => entry.Split(new char[] { ',' }))
                              .Select(toCashReceipt)
                              .Select(toTransactionFromCR);
            };

            Func<string, bool> isEmptyEntry = (entry) => entry.Trim().Length == 0;

            Func<string, bool> isTransaction = (entry) => entry.Length == 6;

            Func<IEnumerable<string>, IEnumerable<string>> trimUnwantedChars = (entries) => entries.Select(val => val.Replace('�', ' '));

            Func<IEnumerable<string>,bool> notHeaderRow = (val) => val.First() != "Date";

            Func<string,string,double> extractTransAmount = (lhs, rhs) => (lhs.Count() == 0) ? Double.Parse(rhs) : Double.Parse(lhs);

            let toTransactionFromE = R.curry((entry) =>
            {
                let currTransaction = exports.ofTransaction(exports.toJSONDate('DD MMM YYYY', R.head(entry)), extractVal(1, entry), extractVal(2, entry), extractTransAmount(extractVal(3, entry), extractVal(4, entry)));
                return currTransaction;
            });

            // genStatementData :: [String] -> [Transaction]
            let genStatementData = R.compose(R.map(toTransactionFromE), R.filter(notHeaderRow), R.map(R.take(5)), R.map(trimUnwantedChars), R.filter(isTransaction), R.map(R.split(',')), R.filter(isEmptyEntry));

            // pattern match to loop through files in directory, calling genCashReceipts() or genStatementData()
            // on matching result, and flatten collection of results into a single set of transactions
            // parserFn :: [(String -> Boolean) -> ([String] -> [Transaction])] -> String -> [Transaction]|ErrorString]
            let parserFn = R.cond([
                [R.contains('cashReceipts'), _srcType => genCashReceipts(transactionSrcData)],
                [R.contains('Statement Download'), _srcType => genStatementData(transactionSrcData)],
                [R.F, R.always(' Invalid source data type')]
            ]);

            return {
            transDataFileName: 'transactions.json',
                transactions: parserFn(srcType)
            };
        });
        
    }
}