using System;
using System.Collections.Generic;

namespace BudgetMgrServiceLib.Models
{
    public class FunctionalComparer<T> : EqualityComparer<T>
    {
        public Func<T, T, bool> comparer { set; get; }
        public Func<T, int> hashCodeGenerator { set; get; }
        public override bool Equals(T x, T y)
        {
            return comparer(x, y);
        }

        public override int GetHashCode(T obj)
        {
            return hashCodeGenerator(obj);
        }
    }
}