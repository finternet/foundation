using FI.Foundation.Dynamic;
using System;
using System.Reflection;

namespace FI.Foundation.Reflection
{
    public class PropertyHelper
    {
        public static void SetPropertyValue(object obj, string propertyName, object propertyValue)
        {
            SetPropertyValue(obj, obj.GetType(), obj.GetType().GetProperty(propertyName), propertyValue);
        }
        public static void SetPropertyValue(object obj, Type type, string propertyName, object propertyValue)
        {
            SetPropertyValue(obj, type, type.GetProperty(propertyName), propertyValue);
        }
        public static void SetPropertyValue(object obj, Type type, PropertyInfo property, object propertyValue)
        {
            SetHandler setPropertyHandler = DynamicMethodCompiler.CreateSetHandler(type,
                    property);
            setPropertyHandler(obj, propertyValue);
        }
        public static object GetPropertyValue(object obj, string propertyName)
        {
            return GetPropertyValue(obj, obj.GetType(), obj.GetType().GetProperty(propertyName));
        }
        public static object GetPropertyValue(object obj, Type type, string propertyName)
        {
            return GetPropertyValue(obj, type, type.GetProperty(propertyName));
        }
        public static object GetPropertyValue(object obj, Type type, PropertyInfo property)
        {
            GetHandler getPropertyHandler = DynamicMethodCompiler.CreateGetHandler(
                type, property);
            return getPropertyHandler(obj);
        }
    }
}
