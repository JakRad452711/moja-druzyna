using System;

namespace moja_druzyna_tests.TestLib
{
    public class TestObjectComparerFactory
    {
        public ObjectsComparer.Comparer<T> CreateDefaultObjectComparer<T>()
        {
            return new ObjectsComparer.Comparer<T>();
        }

        public ObjectsComparer.Comparer<T> CreateShallowObjectComparer<T>()
        {
            ObjectsComparer.Comparer<T> comparer = new ObjectsComparer.Comparer<T>();

            comparer.IgnoreMember(m =>
            {
                System.Reflection.PropertyInfo propertyInfo = typeof(T).GetProperty(m.Name);
                Type propertyType = propertyInfo?.PropertyType;
                bool ignore = true;

                if (propertyType != null)
                    ignore = !propertyType.IsSubclassOf(typeof(ValueType)) && !(propertyType == typeof(string));

                return ignore;
            });

            return comparer;
        }
    }
}
