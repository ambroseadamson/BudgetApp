using BudgetMgrServiceLib.Models;

public class DateFilter
{
    public DateTime StartDate { set; get; }
    public DateTime EndDate { get; set; }
}

public record DateFilterBodyParams(string StartDate, string EndDate);

public record DateFilterAndGroupTypeBodyParams(string StartDate, string EndDate, string GroupType);
