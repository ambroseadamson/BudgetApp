using System;
using System.Collections.Generic;
using System.Linq;
using BudgetMgrServiceLib.Models;

namespace BudgetMgrServiceLib.Processors
{
    public static class FuncPatternExProcessor
    {
        public static FuncPatternEx<T, TResult> With<T, TResult>(this FuncPatternEx<T, TResult> pattern, Predicate<T> condition, Func<T, TResult> result)
        {
            pattern.cases.Add(Tuple.Create(condition, result));
            return pattern;
        }

        public static FuncPatternEx<T, TResult> Else<T, TResult>(this FuncPatternEx<T, TResult> pattern, Func<T, TResult> result)
        {
            if (pattern.elseFunc != null)
                throw new InvalidOperationException("Cannot have multiple else cases");

            pattern.elseFunc = result;
            return pattern;
        }

        public static TResult Do<T, TResult>(this FuncPatternEx<T, TResult> pattern)
        {
            if (pattern.elseFunc != null)
                pattern.cases.Add(Tuple.Create<Predicate<T>, Func<T, TResult>>(x => true, pattern.elseFunc));
            foreach (var item in pattern.cases)
                if (item.Item1(pattern.value))
                    return item.Item2(pattern.value);

            throw new MatchNotFoundException("Incomplete pattern match");
        }

        public static FuncPatternEx<T, TResult> With<T, TResult>(this FuncPatternExContext<T> context, Predicate<T> condition, Func<T, TResult> result)
        {
            var match = new FuncPatternEx<T, TResult>(context.value);
            return match.With(condition, result);
        }

        public static FuncPatternExContext<T> Match<T>(this T value)
        {
            return new FuncPatternExContext<T>(value);
        }
    }
}