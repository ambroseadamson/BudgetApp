using System;
using BudgetMgrServiceLib.Models;
namespace BudgetMgrServiceLib.Processors
{
    public static class PropertyLensProcessor
    {
        public static TResult Map<TObj, TProp, TResult>(this TObj obj, PropertyLens<TObj, TProp, TResult> lens, TProp value)
        {
            TResult result = lens.Setter.Set(obj, value);
            return result;
        }

    }
}