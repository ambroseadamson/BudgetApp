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

}