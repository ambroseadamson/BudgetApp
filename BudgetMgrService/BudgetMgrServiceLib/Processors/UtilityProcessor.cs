using System;
using System.Collections.Generic;
using System.Linq;

namespace BudgetMgrServiceLib.Processors
{
    public static class UtilityProcessor
    {
        public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> collections)
        {
            // filter out the empty collections
            var _collections = collections.Where(collection => collections.Any())
                                          .ToArray();
            foreach (var collection in _collections)
            {
                foreach (var item in collection)
                    yield return item;
            }
        }

        public static IEnumerable<T> AsCollection<T>(this T value) => (new List<T>() { value });

        public static int GenerateUniqueId<T>(this IEnumerable<T> cln, Func<T, int> fnObtainId)
        {
            int id = 1;
            return (cln.Any()) ? cln.Max(val => fnObtainId(val)) + id : 1;
        }

        public static IEnumerable<int> GenerateUniqueIds<T, U>(this IEnumerable<T> srcCln, IEnumerable<U> destCln = null, Func<T, int> fnObtainId = null)
        {
            int _id = 0;
            Func<T, int> _fnObtainId = (fnObtainId == null) ? (val) => _id++ : fnObtainId;
            int lastId = (destCln == null) ? srcCln.Count() : destCln.Count();
            return Enumerable.Range(srcCln.GenerateUniqueId(_fnObtainId), lastId);
        }

        public static void ForEach<T>(this IEnumerable<T> cln, Action<T> action)
        {
            if (!cln.Any())
            {
                return;
            }

            foreach (var val in cln)
            {
                action(val);
            }
        }

        public static void ForEach<T>(this IEnumerable<T> cln, int startIndex, Action<T, int> action)
        {
            if (!cln.Any())
            {
                return;
            }

            var values = Enumerable.Range(startIndex, cln.Count())
                                   .Zip(cln, (index, val) => new { Index = index, Value = val });
            foreach (var val in values)
            {
                action(val.Value, val.Index);
            }
        }
    }
}