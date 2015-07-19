using System;
using System.Reflection;
using System.Reflection.Emit;

namespace FI.Foundation.Dynamic
{
    public delegate object GetHandler(object source);
    public delegate void SetHandler(object source, object value);
    public delegate object InstantiateObjectHandler();

    public sealed class DynamicMethodCompiler
    {
        // DynamicMethodCompiler
        private DynamicMethodCompiler() { }

        // CreateInstantiateObjectDelegate
        public static InstantiateObjectHandler CreateInstantiateObjectHandler(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            ConstructorInfo constructorInfo = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[0], null);
            if (constructorInfo == null)
            {
                throw new TypeAccessException(string.Format(System.Globalization.CultureInfo.CurrentCulture, "The type {0} must declare an empty constructor (the constructor may be private, internal, protected, protected internal, or public).", type));
            }

            DynamicMethod dynamicMethod = new DynamicMethod("InstantiateObject", MethodAttributes.Static | MethodAttributes.Public, CallingConventions.Standard, typeof(object), null, type, true);
            ILGenerator generator = dynamicMethod.GetILGenerator();
            generator.Emit(OpCodes.Newobj, constructorInfo);
            generator.Emit(OpCodes.Ret);
            return (InstantiateObjectHandler)dynamicMethod.CreateDelegate(typeof(InstantiateObjectHandler));
        }
        
        // CreateGetDelegate
        public static GetHandler CreateGetHandler(Type type, PropertyInfo propertyInfo)
        {
            if (propertyInfo == null) throw new ArgumentNullException("propertyInfo");
            if (type == null) throw new ArgumentNullException("type");

            MethodInfo getMethodInfo = propertyInfo.GetGetMethod(true);
            DynamicMethod dynamicGet = CreateGetDynamicMethod(type);
            ILGenerator getGenerator = dynamicGet.GetILGenerator();

            getGenerator.Emit(OpCodes.Ldarg_0);
            getGenerator.Emit(OpCodes.Call, getMethodInfo);
            BoxIfNeeded(getMethodInfo.ReturnType, getGenerator);
            getGenerator.Emit(OpCodes.Ret);
            
            return (GetHandler)dynamicGet.CreateDelegate(typeof(GetHandler));
        }

        // CreateGetDelegate
        public static GetHandler CreateGetHandler(Type type, FieldInfo fieldInfo)
        {
            if (fieldInfo == null) throw new ArgumentNullException("fieldInfo");
            if (type == null) throw new ArgumentNullException("type");

            DynamicMethod dynamicGet = CreateGetDynamicMethod(type);
            ILGenerator getGenerator = dynamicGet.GetILGenerator();

            getGenerator.Emit(OpCodes.Ldarg_0);
            getGenerator.Emit(OpCodes.Ldfld, fieldInfo);
            BoxIfNeeded(fieldInfo.FieldType, getGenerator);
            getGenerator.Emit(OpCodes.Ret);

            return (GetHandler)dynamicGet.CreateDelegate(typeof(GetHandler));
        }

        // CreateSetDelegate
        public static SetHandler CreateSetHandler(Type type, PropertyInfo propertyInfo)
        {
            if (propertyInfo == null) throw new ArgumentNullException("propertyInfo");
            if (type == null) throw new ArgumentNullException("type");

            MethodInfo setMethodInfo = propertyInfo.GetSetMethod(true);
            DynamicMethod dynamicSet = CreateSetDynamicMethod(type);
            ILGenerator setGenerator = dynamicSet.GetILGenerator();

            setGenerator.Emit(OpCodes.Ldarg_0);
            setGenerator.Emit(OpCodes.Ldarg_1);
            UnboxIfNeeded(setMethodInfo.GetParameters()[0].ParameterType, setGenerator);
            setGenerator.Emit(OpCodes.Call, setMethodInfo);
            setGenerator.Emit(OpCodes.Ret);

            return (SetHandler)dynamicSet.CreateDelegate(typeof(SetHandler));
        }

        // CreateSetDelegate
        public static SetHandler CreateSetHandler(Type type, FieldInfo fieldInfo)
        {
            if (fieldInfo == null) throw new ArgumentNullException("fieldInfo");
            if (type == null) throw new ArgumentNullException("type");

            DynamicMethod dynamicSet = CreateSetDynamicMethod(type);
            ILGenerator setGenerator = dynamicSet.GetILGenerator();

            setGenerator.Emit(OpCodes.Ldarg_0);
            setGenerator.Emit(OpCodes.Ldarg_1);
            UnboxIfNeeded(fieldInfo.FieldType, setGenerator);
            setGenerator.Emit(OpCodes.Stfld, fieldInfo);
            setGenerator.Emit(OpCodes.Ret);

            return (SetHandler)dynamicSet.CreateDelegate(typeof(SetHandler));
        }

        // CreateGetDynamicMethod
        private static DynamicMethod CreateGetDynamicMethod(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");
            return new DynamicMethod("DynamicGet", typeof(object), new Type[] { typeof(object) }, type, true);
        }

        // CreateSetDynamicMethod
        private static DynamicMethod CreateSetDynamicMethod(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");
            return new DynamicMethod("DynamicSet", typeof(void), new Type[] { typeof(object), typeof(object) }, type, true);
        }

        // BoxIfNeeded
        private static void BoxIfNeeded(Type type, ILGenerator generator)
        {
            if (type.IsValueType)
            {
                generator.Emit(OpCodes.Box, type);
            }
        }

        // UnboxIfNeeded
        private static void UnboxIfNeeded(Type type, ILGenerator generator)
        {
            if (type.IsValueType)
            {
                generator.Emit(OpCodes.Unbox_Any, type);
            }
        }
    }

    public static class ConverterHelper
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
            if (property != null)
            {
                GetHandler getPropertyHandler = DynamicMethodCompiler.CreateGetHandler(
                    type, property);
                return getPropertyHandler(obj);
            }
            else
                return null;
        }
    }
}