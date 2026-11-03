using BudgetMgrServiceLib.Models;
using BudgetMgrServiceLib.Processors;
using BudgetMgrServiceLib.Generators;
using BudgetMgrWebService.Processors;

public class SummaryResult
{
    public int Id { get; set; }
    public required TransactionCreditDetails Income {set; get;}
    public required DateFilter DateRange {set; get;}

    public required IEnumerable<TransactionSummaryEntry> SummaryData {set; get;}

    public static SummaryResult Of(TransactionContent content, 
                                  IEnumerable<Transaction> allTransactions,
                                  string miscellaenousGroupType)
    {
        Func<IEnumerable<string>> fnObtainGroupNames = () => content.GroupTypeCfg.ToTransNamesEx()
                                                                                 .Select(group => group.GroupName)
                                                                                 .Where(groupName => groupName != "credits")
                                                                                 .Concat(miscellaenousGroupType.AsCollection())
                                                                                 .ToArray();

        var miscTransNamesOfInterest = allTransactions.ToMiscTransNamesOfInterest(content);

        Func<string, TransactionSummaryEntry> fnGenTransData = (_groupType) =>
        {
            var _expenditureGroupNames = (_groupType == miscellaenousGroupType) ? miscTransNamesOfInterest : content.GroupTypeCfg.ToExpenseGroup(_groupType).Names.ToArray();

            return TransactionSummaryEntry.Of(_groupType, allTransactions.GenTransactionSummaryData(content.IncomeGroupNames, _expenditureGroupNames));
        };

        Func<string, TransactionCreditDetails> fnGenIncomeData = (_groupType) =>
        {
            var _expenditureGroupNames = content.GroupTypeCfg.ToExpenseGroup(_groupType).Names.ToArray();

            return allTransactions.GenTransactionSummaryData(content.IncomeGroupNames, _expenditureGroupNames).Income;
        };

        return new SummaryResult()
        {
            Income = fnGenIncomeData(fnObtainGroupNames().First()),
            DateRange = DateFilter.Of(allTransactions),
            SummaryData = fnObtainGroupNames().Select(groupName => fnGenTransData(groupName)).ToArray()
        };
    }

}