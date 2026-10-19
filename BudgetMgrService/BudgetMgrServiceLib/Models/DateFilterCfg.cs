using System;

namespace BudgetMgrServiceLib.Models
{
    public class DateFilterCfg
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool UseDateFilter { get; set; }

        public static DateFilterCfg Of(DateTime startDate, DateTime endDate, bool useDateFilter)
        {
            return new DateFilterCfg()
            {
                StartDate = startDate,
                EndDate = endDate,
                UseDateFilter = useDateFilter
            };
        }

    }
}