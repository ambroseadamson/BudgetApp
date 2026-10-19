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

    public class PropertySetter<TObj, TProp>
    {
        public Action<TObj, TProp> Set { get; private set; }

        public static PropertySetter<TObj, TProp> Of(Action<TObj, TProp> setter)
        {
            return new PropertySetter<TObj, TProp>()
            {
                Set = setter,
            };
        }

    }

    public class PropertyLens<TObj, TProp>
    {
        public PropertySetter<TObj, TProp> Setter { get; private set; }
        public PropertyGetter<TObj, TProp> Getter { get; private set; }

        public static PropertyLens<TObj, TProp> Of(Func<TObj, TProp> getter,
                                                   Action<TObj, TProp> setter)
        {
            return new PropertyLens<TObj, TProp>()
            {
                Setter = PropertySetter<TObj, TProp>.Of(setter),
                Getter = PropertyGetter<TObj, TProp>.Of(getter)
            };
        }
    }

}