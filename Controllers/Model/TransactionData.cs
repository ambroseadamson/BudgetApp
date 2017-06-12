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


        public DateTime DateStamp { set; get; }
        public string TransType { set; get; }
        public string Description { set; get; }
        public double Amount { set; get; }
    }


    public class CashReceipt
    {
        public CashReceipt()
        {
        }
        public DateTime Date { get; set; }
        public string Desc { get; set; }
        public double Total { get; set; }
        public double CashPaid { get; set; }

    }

    public class TransactionResults
    {
        public TransactionResults()
        {
        }

        public string ResultName { get; set; }
        public IEnumerable<Transaction> Results { get; set; }
    }

    public class FuncPattern<T,U,V>
    {
        public Func<T, bool> Pattern { get; set; }
        public Func<U, V> Action { get; set; }
        public FuncPattern(Func<T,bool> pattern, Func<U,V> action)
        {
            Pattern = pattern;
            Action = action;
        }
    }

    public static class FuncPatternExt
    {
        public static IEnumerable<FuncPattern<T, U, V>> Builder<T,U,V>()
        {
            return new List<FuncPattern<T, U, V>>() { };
        }
        
        public static IEnumerable<FuncPattern<T, U, V>> AddPattern<T, U, V>(this IEnumerable<FuncPattern<T, U, V>> patterns, Func<T, bool> pattern, Func<U, V> action)
        {
            return patterns.Append(new FuncPattern<T, U, V>(pattern, action));
        }

        public static IEnumerable<FuncPattern<T, U, V>> AddDefaultPattern<T, U, V>(this IEnumerable<FuncPattern<T, U, V>> patterns)
        {
            return patterns.Append(new FuncPattern<T, U, V>((T patternVal) => true, (U actionVal) => default(V)));
        }

        public static IEnumerable<FuncPattern<T, U, V>> Build<T, U, V>(this IEnumerable<FuncPattern<T, U, V>> patterns)
        {
            return patterns;
        }

        public static Func<U, V> Match<T,U,V>(this IEnumerable<FuncPattern<T,U,V>> patterns, T patternVal)
        {
            return patterns.First(entry => entry.Pattern(patternVal) == true).Action;
        }

        public static V Do<T,U,V>(this IEnumerable<FuncPattern<T, U, V>> patterns, T patternVal, U actionVal)
        {
            return Match(patterns, patternVal)(actionVal);
        }
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
                    Date = toDateStamp(entry.First()),
                    Desc = extractVal(1, entry),
                    Total = Double.Parse(extractVal(2, entry)),
                    CashPaid = Double.Parse(extractVal(3, entry))
                };
            };

            Func<CashReceipt, Transaction> toTransactionFromCR = (cashReceipt) =>
            {
                return new Transaction()
                {
                    DateStamp = cashReceipt.Date,
                    TransType = "CASH RECEIPT",
                    Description = cashReceipt.Desc,
                    Amount = cashReceipt.Total
                };
            };

            Func<IEnumerable<string>, IEnumerable<Transaction>> genCashReceipts = (entries) =>
            {
                return entries.Select(entry => entry.Split(new char[] { ',' }))
                              .Select(toCashReceipt)
                              .Select(toTransactionFromCR);
            };

            Func<string, bool> isEmptyEntry = (entry) => entry.Trim().Length == 0;

            Func<IEnumerable<string>, bool> isTransaction = (entry) => entry.Count() == 6;

            Func<IEnumerable<string>, IEnumerable<string>> trimUnwantedChars = (entries) => entries.Select(val => val.Replace('�', ' '));

            Func<IEnumerable<string>,bool> notHeaderRow = (val) => val.First() != "Date";

            Func<string,string,double> extractTransAmount = (lhs, rhs) => (lhs.Count() == 0) ? Double.Parse(rhs) : Double.Parse(lhs);

            // toTransactionFromE :: entry -> Transaction
            Func<IEnumerable<string>,Transaction> toTransactionFromE = (entry) => {
                return new Transaction() {
                    DateStamp=toDateStamp(entry.First()),
                    TransType= extractVal(1, entry),
                    Description= extractVal(2, entry),
                    Amount= extractTransAmount(extractVal(3, entry), extractVal(4, entry))
                };
            };

            Func<IEnumerable<string>, IEnumerable<Transaction>> genStatementData = (entries) =>
             {
                 return entries.Where(entry => isEmptyEntry(entry) == false)
                               .Select(entry => entry.Split(new char[] { '\'' }))
                               .Where(isTransaction)
                               .Select(trimUnwantedChars)
                               .Select(entry => entry.Take(5))
                               .Where(notHeaderRow)
                               .Select(toTransactionFromE);
             };

            // pattern match to loop through files in directory, calling genCashReceipts() or genStatementData()
            // on matching result, and flatten collection of results into a single set of transactions
            // parserFn :: [(String -> Boolean) -> ([String] -> [Transaction])] -> String -> [Transaction]|ErrorString]5x
            var patterns = FuncPatternExt.Builder<string, IEnumerable<string>, IEnumerable<Transaction>>()
                                .AddPattern((_srcType) => _srcType.Contains("cashReceipts"), (srcData) => genCashReceipts(srcData))
                                .AddPattern((_srcType) => _srcType.Contains("Statement Download"), (srcData) => genStatementData(srcData))
                                .AddDefaultPattern()
                                .Build();

            
            return new TransactionResults(){
                ResultName ="transactions.json",
                Results= patterns.Do(srcType, transactionSrcData)
            };
        }
    }
}