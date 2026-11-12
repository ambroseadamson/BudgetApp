using System;

namespace BudgetMgrServiceLib.Models
{
    public record Transaction(long Id, DateTime DateStamp, string TransType, string Description, double Amount);
}