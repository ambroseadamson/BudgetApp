using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BudgetMgrServiceLib.Models;
using BudgetMgrServiceLib.Processors;

namespace BudgetMgrServiceLib.Generators
{
    public static class TransactionGenerator
    {
        public static TransactionResults GenTransactionData(this IEnumerable<string> transactionSrcData,
                                                            TestDataCfg testDataCfg, GroupTypesCfgEx groupTypesCfg, string srcType,
                                                            Func<IEnumerable<Transaction>, IEnumerable<Transaction>> applyUniqueIds)
        {

            Func<int, IEnumerable<string>, string> extractVal = (n, entries) => entries.Skip(n).First();

            Func<string, DateTime> toDateStamp = (val) =>
            {
                DateTime dt;
                var result = DateTime.TryParse(val, out dt);
                return result ? dt :DateTime.MinValue;
            };

            Func<string, bool> isEmptyEntry = (entry) => entry.Trim().Length == 0;

            Func<IEnumerable<string>, bool> isTransaction = (entry) => entry.Count() == 6;

            Func<IEnumerable<string>, IEnumerable<string>> trimUnwantedChars = (entries) => entries.Select(val => val.Replace('�', ' '));

            Func<IEnumerable<string>, bool> notHeaderRow = (val) => val.First() != "Date";

            Func<string, string, double> extractTransAmount = (lhs, rhs) => (lhs.Count() == 0) ? Double.Parse(rhs) : Double.Parse(lhs);

            Func<IEnumerable<string>, Transaction> toTransactionFromE = (entry) =>
            {
                return Transaction.Of(0, toDateStamp(entry.First()), extractVal(1, entry).ToUpper(),
                                         extractVal(2, entry).ToUpper(), extractTransAmount(extractVal(3, entry),
                                         extractVal(4, entry)));
            };

            string[] debitCreditTypes = { "BANK CREDIT", "DIRECT DEBIT" };

            Func<string, bool> isDebitCreditType = (val) => debitCreditTypes.Any(type => val.ToUpper().Contains(type));

            Func<Transaction, IEnumerable<string>> getTypeParts = (transaction) => transaction.TransType.Split(new char[] { ' ' });
            Func<Transaction, string> getDebitCreditType = (transaction) => string.Join(" ", getTypeParts(transaction).Take(2).ToArray());
            Func<Transaction, string> getDebitCreditDescription = (transaction) => string.Join(" ", getTypeParts(transaction).Skip(2).ToArray()).Trim();

            Func<Transaction, Transaction> fixDebitCreditDetails = (transaction) =>
            {
                var typeRequiresFixing = isDebitCreditType(transaction.TransType);
                var descRequiresFixing = isDebitCreditType(transaction.Description);
                var type = typeRequiresFixing ? getDebitCreditType(transaction) : transaction.TransType;
                var desc = descRequiresFixing ? getDebitCreditDescription(transaction) : transaction.Description;
                return Transaction.Of(transaction.Id, transaction.DateStamp, type, desc, transaction.Amount);
            };

            Func<IEnumerable<string>, IEnumerable<Transaction>> genStatementData = (entries) =>
            {
                return applyUniqueIds(entries.Where(entry => isEmptyEntry(entry) == false)
                                             .Select(entry => entry.Replace("\"", "").Split(new char[] { ',' }))
                                             .Where(isTransaction)
                                             .Select(trimUnwantedChars)
                                             .Select(entry => entry.Take(5))
                                             .Where(notHeaderRow)
                                             .Select(toTransactionFromE)
                                             .Select(fixDebitCreditDetails));
            };



            // pattern match calling genCashReceipts() or genStatementData() on matching result
            var results = srcType.Match()
                                 .With(_srcType => _srcType.Contains("Statement Download"), (_srcType) => genStatementData(transactionSrcData))
                                 .Do()
                                 .ToArray();

            return TransactionResults.Of("transactions.json",
                                         results,
                                         results.GenDateFilterCfgURLParam(testDataCfg.DateFilter.UseDateFilter));
        }
    }
}