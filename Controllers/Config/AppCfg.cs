using System;
using System.Collections.Generic;
using System.Linq;

namespace BudgetApp.Controllers.Config
{
    public class TransNames 
    {
        public TransNames() {

        }

        public string groupType {set; get;}
        public IEnumerable<string> names {set; get;}
    }

    public class AppCfg
    {
        public AppCfg() {

        }

        public int numberOfTransactions { set; get;}
        public IEnumerable<string> months {set; get; }
        public IEnumerable<string> years {set; get; }
        public IEnumerable<TransNames> transNames {set; get; }
        public IEnumerable<string> creditNamesOfInterest {set; get; }
        public IEnumerable<string> debitNamesOfInterest {set; get; }
        public IEnumerable<char> charsToRemove {set; get; }
        public IEnumerable<string> creditTypesOfInterest {set; get; }
        public IEnumerable<string> debitTypesOfInterest {set; get; }
        public IEnumerable<string> transTypes {set; get; }
    }

    public static class AppCfgExt
    {
        public static string ToTransType(this AppCfg appCfg, int index) => appCfg.transTypes.ElementAt(index);

        public static string ToTransName(this AppCfg appCfg, int index) => appCfg.transNames.ElementAt(index).groupType;

        public static double ToTransAmount(this AppCfg appCfg, int pounds, int pence) => Double.Parse($"{pounds}.{pence}");

        public static int ToYearVal(this Random rnd, AppCfg appCfg)
        {
            string[] years = appCfg.years.ToArray();
            return Int32.Parse(years[ToRandomSrcVal(rnd, 0, appCfg.years.Count() - 1)]);
        }

        public static int ToRandomSrcVal(this Random rnd, int min, int max) => rnd.Next(min, max);
    }
}