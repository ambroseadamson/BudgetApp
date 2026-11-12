using Optional;
using System.Collections.Generic;

namespace BudgetMgrServiceLib.Models
{
    public record TransactionType(long Id, string TransType, string TypeName);

    public record TransactionName(long Id, string TransType, string Name);


    public record TransactionGroupEx(long Id, string GroupName, long GroupTypeFK);

    public record TransactionNameEx(long Id, long TransGroupFK, long TransNameFK);

    public record TransactionGroupCfg(long Id, string GroupName, string GroupType, IEnumerable<string> Names);

    public record UpdateTransactionName(long Id, string Name);

    public record GroupUpdateSourceReferences(
        Option<IEnumerable<TransactionGroupEx>> TransGroupsToAdd,
        Option<IEnumerable<TransName>> TransNamesToAdd,
        Option<IEnumerable<long>> NameRefsToRemove,
        Option<IEnumerable<TransactionNameEx>> NameRefsToAdd
    );

    public record UpdatedGroupSources(
        IEnumerable<TransactionNameEx> UpdatedNameRefs,
        Option<IEnumerable<TransName>> UpdatedTransNames,
        IEnumerable<TransactionGroupEx> UpdatedTransGroups
    );

}