using System;
using System.Collections.Generic;
using System.Linq;

namespace BudgetMgrServiceLib.Models
{
    public record CreditTypeRef(
        long Id,
        long TransTypeFK
    );

    public record DebitTypeRef(
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

    public record FilePath(
        long Id,
        string Value
    );

    public record GroupTypesCfgEx(
        IEnumerable<CreditTypeRef> CreditTypesOfInterest,
        IEnumerable<DebitTypeRef> DebitTypesOfInterest,
        IEnumerable<TransName> TransNames,
        IEnumerable<TransType> TransTypes,
        IEnumerable<TransGroupType> TransGroupTypes,
        IEnumerable<TransactionGroupEx> TransGroups,
        IEnumerable<TransactionNameEx> NameRefs
    );

}