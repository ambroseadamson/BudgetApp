using System;
using System.Collections.Generic;
using System.Linq;

namespace BudgetMgrServiceLib.Models
{
    public class TestDataCfg
    {
        public int NumberOfTransactions { get; set; }

        public IEnumerable<string> MiscTransactionNames { set; get; }

        public double MiscTransactionValue { set; get; }

        public IEnumerable<string> Months { set; get; }
        public IEnumerable<string> Years { set; get; }
        public IEnumerable<char> CharsToRemove { set; get; }
        public DateFilterCfg DateFilter { get; set; }
    }
}