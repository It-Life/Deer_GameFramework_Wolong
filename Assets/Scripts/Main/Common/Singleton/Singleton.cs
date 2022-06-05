// ================================================
//描 述 :  
//作 者 : 杜鑫 
//创建时间 : 2022-03-16 15-11-15  
//修改作者 : 杜鑫 
//修改时间 : 2022-03-16 15-11-15  
//版 本 : 0.1 
// ===============================================

// 由于单例基类不能实例化，故设计为抽象类
using System;

public abstract class Singleton<T> where T : class
{
    class Nested
    {
        // 创建模板类实例，参数2设为true表示支持私有构造函数
        internal static readonly T instance = Activator.CreateInstance(typeof(T), true) as T;
    }
    public static T Instance { get { return Nested.instance; } }
}