using BudgetMgrServiceLib.Models;

public record TransactionContent(GroupTypesCfgEx GroupTypeCfg, IEnumerable<Transaction> Transactions, IEnumerable<string> IncomeGroupNames);
