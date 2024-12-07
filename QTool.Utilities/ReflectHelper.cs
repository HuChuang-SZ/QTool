using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QTool
{
    public static class ReflectHelper
    {
        public static Func<object, object> GetPropertyValueFunc(Type type, string propertyName)
        {
            var property = type.GetProperty(propertyName);
            var target = Expression.Parameter(typeof(object));
            var castTarget = Expression.Convert(target, type);
            var getPropertyValue = Expression.Property(castTarget, property);
            var castPropertyvalue = Expression.Convert(getPropertyValue, typeof(object));
            return Expression.Lambda<Func<object, object>>(castPropertyvalue, target).Compile();
        }

        public static Action<object, object> SetPropertyValueFunc(Type type, string propertyName)
        {
            var property = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            var objectObj = Expression.Parameter(typeof(object), "obj");
            var objectValue = Expression.Parameter(typeof(object), "value");
            var classObj = Expression.Convert(objectObj, type);
            var classValue = Expression.Convert(objectValue, property.PropertyType);
            var classFunc = Expression.Call(classObj, property.GetSetMethod(), classValue);
            var setProperty = Expression.Lambda<Action<object, object>>(classFunc, objectObj, objectValue).Compile();
            return setProperty;
        }
    }
}
