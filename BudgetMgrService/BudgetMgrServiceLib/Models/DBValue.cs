namespace BudgetMgrServiceLib.Models
{
    public record CreditTypeIndex(
        long Id,
        long TransTypeFK
    );

    public record DebitTypeIndex(
        long Id,
        long TransTypeFK
    );


    public record TransName(
        long Id,
        string Name
    );

    public record TransType(
        long Id,
        string Type
    );

    public record TransGroupType(
        long Id,
        string Type
    );

    public record TransactionGroupIndex(long Id, string GroupName, long GroupTypeFK);

    public record TransactionNameIndex(long Id, long TransGroupFK, long TransNameFK);
}