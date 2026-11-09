using BudgetMgrServiceLib.Models;
using BudgetMgrServiceLib.Processors;
using BudgetMgrWebService.Processors;
using Optional;

public class TransactionContent
{
    public required GroupTypesCfgEx GroupTypeCfg {set; get;}
    public required IEnumerable<Transaction> Transactions {set; get;}

    public required IEnumerable<string> IncomeGroupNames {set; get;}

    public static TransactionContent Of(TransactionContext dbContext)
    {
        var groupTypeCfg = dbContext.ToGroupTypeCfgEx();
        var transactions = Option.Some<IEnumerable<Transaction>>(dbContext.Transactions.ToArray());
        

        var incomeGroupNames = groupTypeCfg.ToTotalIncomeGroup(transactions).Names.ToArray();

        return new TransactionContent()
        {
            GroupTypeCfg = groupTypeCfg,
            Transactions = transactions.ValueOr(Enumerable.Empty<Transaction>()),
            IncomeGroupNames = incomeGroupNames,
        };
    }
}