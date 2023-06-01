// ================================================
//描 述:
//作 者:杜鑫
//创建时间:2022-06-10 20-32-12
//修改作者:杜鑫
//修改时间:2022-06-10 20-32-12
//版 本:0.1 
// ===============================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Please modify the description.
/// </summary>
public static class TypeUtils
{
    /// <summary>
    /// 在运行时热更程序集中获取指定基类的所有子类的名称。
    /// </summary>
    /// <param name="typeBase">基类类型。</param>
    /// <returns>指定基类的所有子类的名称。</returns>
    public static string[] GetRuntimeTypeNames(System.Type typeBase, List<Assembly> assemblys) 
    {
        string[] runtimeAssemblyNames = new string[] { "Main.Runtime" };
        //string assemblyName = "HotfixFramework.Runtime";
        List<string> typeNames = new List<string>();
        typeNames.AddRange(GetTypeNames(typeBase, runtimeAssemblyNames));
        typeNames.AddRange(GetTypeNames(typeBase, assemblys));
        return typeNames.ToArray();
    }
    private static string[] GetTypeNames(System.Type typeBase, string[] assemblyNames)
    {
        List<string> typeNames = new List<string>();
        foreach (string assemblyName in assemblyNames)
        {
            Assembly assembly = null;
            try
            {
                assembly = Assembly.Load(assemblyName);
            }
            catch
            {
                continue;
            }

            if (assembly == null)
            {
                continue;
            }

            System.Type[] types = assembly.GetTypes();
            foreach (System.Type type in types)
            {
                if (type.IsClass && !type.IsAbstract && typeBase.IsAssignableFrom(type))
                {
                    typeNames.Add(type.FullName);
                }
            }
        }

        typeNames.Sort();
        return typeNames.ToArray();
    }
    public static string[] GetTypeNames(System.Type typeBase, List<Assembly> assemblys) 
    {
        List<string> typeNames = new List<string>();
        for (int i = 0; i < assemblys.Count; i++)
        {
            Assembly assembly = assemblys[i];
            System.Type[] types = assembly.GetTypes();
            foreach (System.Type type in types)
            {
                if (type.IsClass && !type.IsAbstract && typeBase.IsAssignableFrom(type))
                {
                    typeNames.Add(type.FullName);
                }
            }
        }
        return typeNames.ToArray();
    }
    
}