using System;

namespace BudgetMgrServiceLib.Models
{
    public record DateFilterCfg(DateTime StartDate, DateTime EndDate, bool UseDateFilter);
}