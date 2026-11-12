using System;
namespace BudgetMgrServiceLib.Models
{
    public class PropertyGetter<TObj, TProp>
    {
        public Func<TObj, TProp> Get { get; private set; }

        public static PropertyGetter<TObj, TProp> Of(Func<TObj, TProp> getter)
        {
            return new PropertyGetter<TObj, TProp>()
            {
                Get = getter,
            };
        }
    }

    public class PropertySetter<TObj, TProp, TResult>
    {
        public Func<TObj, TProp, TResult> Set { get; private set; }

        public static PropertySetter<TObj, TProp, TResult> Of(Func<TObj, TProp, TResult> setter)
        {
            return new PropertySetter<TObj, TProp, TResult>()
            {
                Set = setter,
            };
        }

    }

    public class PropertyLens<TObj, TProp, TResult>
    {
        public PropertySetter<TObj, TProp, TResult> Setter { get; private set; }
        public PropertyGetter<TObj, TProp> Getter { get; private set; }

        public static PropertyLens<TObj, TProp, TResult> Of(Func<TObj, TProp> getter,
                                                             Func<TObj, TProp, TResult> setter)
        {
            return new PropertyLens<TObj, TProp, TResult>()
            {
                Setter = PropertySetter<TObj, TProp, TResult>.Of(setter),
                Getter = PropertyGetter<TObj, TProp>.Of(getter)
            };
        }
    }

}