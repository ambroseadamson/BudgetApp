using BudgetMgrServiceLib.Models;

public class DateFilter
{
    public int Id { get; set; }
    public DateTime StartDate { set; get; }
    public DateTime EndDate { get; set; }

    public static DateFilter Of(IEnumerable<Transaction> transactions)
    {
        var sortedTransactions = transactions.OrderBy(transaction => transaction.DateStamp);
        return new DateFilter()
        {
            StartDate = sortedTransactions.First().DateStamp,
            EndDate = sortedTransactions.Last().DateStamp
        };
    }
}

public class DateFilterBodyParams
{
    public required string StartDate { set; get; }
    public required string EndDate { get; set; }
    public static DateFilterBodyParams Of(string startDate, string endDate)
    {
        return new DateFilterBodyParams()
        {
            StartDate = startDate,
            EndDate = endDate
        };
    }

}

public class DateFilterAndGroupTypeBodyParams : DateFilterBodyParams
{
    public required string GroupType { get; set; }
    public static DateFilterAndGroupTypeBodyParams Of(string startDate, string endDate, string groupType)
    {
        return new DateFilterAndGroupTypeBodyParams()
        {
            StartDate = startDate,
            EndDate = endDate,
            GroupType = groupType
        };
    }

}
