using System;
using System.Collections.Generic;
using System.Linq;
using BudgetMgrServiceLib.Models;

namespace BudgetMgrServiceLib.Models
{
    public class MatchNotFoundException : Exception
    {
        public MatchNotFoundException(string message) : base(message) { }
    }

    public class FuncPatternEx<T, TResult>
    {
        public readonly T value;
        public readonly List<Tuple<Predicate<T>, Func<T, TResult>>> cases = new List<Tuple<Predicate<T>, Func<T, TResult>>>();
        public Func<T, TResult> elseFunc;

        internal FuncPatternEx(T value)
        {
            this.value = value;
        }

    }

    public class FuncPatternExContext<T>
    {
        public readonly T value;

        internal FuncPatternExContext(T value)
        {
            this.value = value;
        }

    } 
}