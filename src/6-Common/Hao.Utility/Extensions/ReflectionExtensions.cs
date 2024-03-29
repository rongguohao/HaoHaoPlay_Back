﻿using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Hao.Utility
{
    public static class ReflectionExtensions
    {
        public static Type[] GetGenericArguments(this Type type)
        {
            var reval = type.GetTypeInfo().GetGenericArguments();
            return reval;
        }
        public static bool IsGenericType(this Type type)
        {
            var reval = type.GetTypeInfo().IsGenericType;
            return reval;
        }
        public static PropertyInfo[] GetProperties(this Type type)
        {
            var reval = type.GetTypeInfo().GetProperties();
            return reval;
        }
        public static PropertyInfo GetProperty(this Type type, string name)
        {
            var reval = type.GetTypeInfo().GetProperty(name);
            return reval;
        }

        public static FieldInfo GetField(this Type type, string name)
        {
            var reval = type.GetTypeInfo().GetField(name);
            return reval;
        }

        public static bool IsEnum(this Type type)
        {
            var reval = type.GetTypeInfo().IsEnum;
            return reval;
        }

        public static bool IsNullableEnum(this Type t)
        {
            Type u = Nullable.GetUnderlyingType(t);
            return (u != null) && u.IsEnum;
        }

        //public static bool IsNullableEnum2(Type t)  可为空枚举 是 Nullable<> 泛型
        //{
        //    return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>) && t.GetGenericArguments()[0].IsEnum;
        //}


        public static MethodInfo GetMethod(this Type type, string name)
        {
            var reval = type.GetTypeInfo().GetMethod(name);
            return reval;
        }

        public static MethodInfo GetMethod(this Type type, string name, Type[] types)
        {
            var reval = type.GetTypeInfo().GetMethod(name, types);
            return reval;
        }
        public static ConstructorInfo GetConstructor(this Type type, Type[] types)
        {
            var reval = type.GetTypeInfo().GetConstructor(types);
            return reval;
        }

        public static bool IsValueType(this Type type)
        {
            return type.GetTypeInfo().IsValueType;
        }

        public static bool IsEntity(this Type type)
        {
            return type.GetTypeInfo().IsClass;
        }

        public static Type ReflectedType(this MethodInfo method)
        {
            return method.ReflectedType;
        }

        public static bool IsClass(this Type type)
        {
            return type != H_Type.StringType && type.IsEntity();
        }


        /// <summary>
        /// 判断是否时异步方法或者异步的泛型方法
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static bool IsAsyncMethod(MethodInfo method)
        {
            return method.ReturnType == typeof(Task) || (method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>));
        }
    }
}
