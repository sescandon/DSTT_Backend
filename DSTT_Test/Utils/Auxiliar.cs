using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSTT_Test.Utils
{
    public static class AuxiliarClass
    {

        public static object? GetNestedPropertyValue(object obj, params string[] propertyNames)
        {
            object value = obj;
            foreach (var prop in propertyNames)
            {
                var property = value.GetType().GetProperty(prop);
                if (property == null)
                    return null;
                value = property.GetValue(value)!;
                if (value == null)
                    return null;
            }
            return value;
        }
    }
}
