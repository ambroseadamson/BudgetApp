using System;
using System.Collections.Generic;
using System.Linq;
using BudgetMgrServiceLib.Models;

namespace BudgetMgrServiceLib.Models
{
    public class Transaction
    {
        public long Id { get; set; }
        public DateTime DateStamp { get; set; }
        public string TransType { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }

        public static Transaction Of(long id, DateTime dateStamp, string transType, string description, double amount)
        {
            return new Transaction()
            {
                Id = id,
                DateStamp = dateStamp,
                TransType = transType,
                Description = description,
                Amount = amount
            };
        }
    }
}