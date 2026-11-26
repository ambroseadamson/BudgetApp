using Optional;
using System.Collections.Generic;

namespace BudgetMgrServiceLib.Models
{
    public record TransactionType(long Id, string TransType, string TypeName);

    public record TransactionName(long Id, string TransType, string Name);


    public record TransactionGroupCfg(long Id, string GroupName, string GroupType, IEnumerable<string> Names);

    public record UpdateTransactionName(long Id, string Name);

    public record GroupTypesCfgEx(
        IEnumerable<CreditTypeIndex> CreditTypesOfInterest,
        IEnumerable<DebitTypeIndex> DebitTypesOfInterest,
        IEnumerable<TransName> TransNames,
        IEnumerable<TransType> TransTypes,
        IEnumerable<TransGroupType> TransGroupTypes,
        IEnumerable<TransactionGroupIndex> TransGroups,
        IEnumerable<TransactionNameIndex> NameRefs
    );

    public record GroupUpdateSourceReferences(
        Option<IEnumerable<TransactionGroupIndex>> TransGroupsToAdd,
        Option<IEnumerable<TransName>> TransNamesToAdd,
        Option<IEnumerable<long>> NameRefsToRemove,
        Option<IEnumerable<TransactionNameIndex>> NameRefsToAdd
    );

    public record UpdatedGroupSources(
        IEnumerable<TransactionNameIndex> UpdatedNameRefs,
        Option<IEnumerable<TransName>> UpdatedTransNames,
        IEnumerable<TransactionGroupIndex> UpdatedTransGroups
    );

}