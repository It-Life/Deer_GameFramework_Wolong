using System;
using System.Collections;
using System.Collections.Generic;

public static class TypeUtils
{
    /// <summary>
    ///     判断是否是List类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsList(this Type type)
    {
        if (typeof(IList).IsAssignableFrom(type)) return true;
        foreach (var it in type.GetInterfaces())
            if (it.IsGenericType && typeof(IList<>) == it.GetGenericTypeDefinition())
                return true;
        return false;
    }

    /// <summary>
    ///     判断是否是数组类型 注意这个也可以判断List
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsArray(this Type type)
    {
        if (typeof(Array).IsAssignableFrom(type)) return true;
        foreach (var it in type.GetInterfaces())
            if (it.IsGenericType && typeof(Array) == it.GetGenericTypeDefinition())
                return true;
        return false;
    }

    /// <summary>
    ///     判断是否是字典类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsDictionary(this Type type)
    {
        if (typeof(IDictionary).IsAssignableFrom(type)) return true;
        foreach (var it in type.GetInterfaces())
            if (it.IsGenericType && typeof(IDictionary) == it.GetGenericTypeDefinition())
                return true;
        return false;
    }

    /// <summary>
    ///     判断是否是枚举
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsEnumerable(this Type type)
    {
        if (type.IsArray) return true;
        if (typeof(IEnumerable).IsAssignableFrom(type)) return true;
        foreach (var it in type.GetInterfaces())
            if (it.IsGenericType && typeof(IEnumerable<>) == it.GetGenericTypeDefinition())
                return true;
        return false;
    }
}