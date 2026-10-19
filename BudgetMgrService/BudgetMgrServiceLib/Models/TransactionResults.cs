using System;
using System.Collections.Generic;
using System.Linq;
using BudgetMgrServiceLib.Models;

namespace BudgetMgrServiceLib.Models
{
    public class TransactionResults
    {
        public string ResultName { get; set; }
        public IEnumerable<Transaction> Results { get; set; }
        public string DateRangeURLParam { get; set; }

        public static TransactionResults Of(string resultName, IEnumerable<Transaction> results, string dateRangeURLParam)
        {
            return new TransactionResults()
            {
                ResultName = resultName,
                Results = results,
                DateRangeURLParam = dateRangeURLParam
            };
        }
    }

}