using System;
using System.Collections.Generic;
using System.Linq;

namespace BudgetMgrServiceLib.Models
{
    public class CreditTypeRef
    {
        public long Id { get; set; }
        public long TransTypeFK { set; get; }
    }

    public class DebitTypeRef
    {
        public long Id { get; set; }
        public long TransTypeFK { set; get; }
    }


    public class TransName
    {
        public long Id { get; set; }
        public string Name { set; get; }
    }

    public class TransType
    {
        public long Id { get; set; }
        public string Type { set; get; }
    }

    public class TransGroupType
    {
        public long Id { get; set; }
        public string Type { set; get; }
    }

    public class FilePath
    {
        public long Id { get; set; }
        public string Value { set; get; }
    }

    public class GroupTypesCfgEx
    {
        public IEnumerable<CreditTypeRef> CreditTypesOfInterest { set; get; }
        public IEnumerable<DebitTypeRef> DebitTypesOfInterest { set; get; }
        public IEnumerable<TransName> TransNames { set; get; }
        public IEnumerable<TransType> TransTypes { set; get; }

        public IEnumerable<TransGroupType> TransGroupTypes { set; get; }

        public IEnumerable<TransactionGroupEx> TransGroups { set; get; }

        public IEnumerable<TransactionNameEx> NameRefs { set; get; }
    }

}