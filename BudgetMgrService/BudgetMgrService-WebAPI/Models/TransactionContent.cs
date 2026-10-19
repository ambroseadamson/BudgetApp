using System;
using System.Collections.Generic;
using System.Linq;
using BudgetMgrService_WebAPI.Models;
using BudgetMgrService_WebAPI.Processors;
using BudgetMgrServiceLib.Models;
using BudgetMgrServiceLib.Processors;
using Optional;

public class TransactionContent
{
    public GroupTypesCfgEx GroupTypeCfg {set; get;}
    public IEnumerable<Transaction> Transactions {set; get;}

    public IEnumerable<string> IncomeGroupNames {set; get;}

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