using Optional;
using System.Collections.Generic;

namespace BudgetMgrServiceLib.Models
{
    public class TransactionType
    {
        public long Id { get; set; }
        public string TransType { set; get; }
        public string TypeName { set; get; }
    }

    public class TransactionName
    {
        public long Id { get; set; }
        public string TransType { set; get; }
        public string Name { set; get; }
    }


    public class TransactionGroupEx
    {
        public long Id { get; set; }
        public string GroupName { set; get; }
        public long GroupTypeFK { set; get; }
    }

    public class TransactionNameEx
    {
        public long Id { get; set; }
        public long TransGroupFK { set; get; }
        public long TransNameFK { set; get; }
    }

    public class TransactionGroupCfg
    {
        public long Id { get; set; }
        public string GroupName { set; get; }
        public string GroupType { set; get; }
        public IEnumerable<string> Names { set; get; }
    }

    public class UpdateTransactionName
    {
        public long Id { get; set; }
        public string Name { set; get; }
    }

    public class GroupUpdateSourceReferences
    {
        public Option<IEnumerable<TransactionGroupEx>> TransGroupsToAdd { get; set; }
        public Option<IEnumerable<TransName>> TransNamesToAdd { get; set; }
        public Option<IEnumerable<long>> NameRefsToRemove { get; set; }
        public Option<IEnumerable<TransactionNameEx>> NameRefsToAdd { get; set; }

    }

    public class UpdatedGroupSources
    {
        public IEnumerable<TransactionNameEx> UpdatedNameRefs { get; set; }
        public Option<IEnumerable<TransName>> UpdatedTransNames { get; set; }
        public IEnumerable<TransactionGroupEx> UpdatedTransGroups { get; set; }

    }

}