using System;
using System.Collections.Generic;
using System.Linq;

namespace BudgetMgrServiceLib.Models
{
    public class AttributeValue<T>
    {
        public long Id { get; set; }
        public T Value { set; get; }
    }
}