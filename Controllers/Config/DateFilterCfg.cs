using System;

namespace BudgetApp.Controllers.Config
{
    public class DateFilterCfg
    {
        public DateTime startDate { set; get; }
        public DateTime endDate { set; get; }
        public bool useDateFilter { get; set; }

        public DateFilterCfg()
        {
        }
    }
}