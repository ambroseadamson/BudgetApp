using System;
using BudgetMgrServiceLib.Models;
namespace BudgetMgrServiceLib.Processors
{
    public static class PropertyLensProcessor
    {
        public static TObj Map<TObj, TProp>(this TObj obj, PropertyLens<TObj, TProp> lens, TProp value)
        {
            lens.Setter.Set(obj, value);
            return obj;
        }

    }
}