// ================================================
//描 述 :  
//作 者 : 杜鑫 
//创建时间 : 2021-07-09 08-18-03  
//修改作者 : 杜鑫 
//修改时间 : 2021-07-09 08-18-03  
//版 本 : 0.1 
// ===============================================

using System;
using System.Collections.Generic;

namespace HotfixFramework.Runtime
{
    public static class DataConfig
    {
        public static readonly List<Type> Names = new List<Type>()
        {
            typeof(SoundsConfigDataInfo),
            typeof(UIFormsConfigDataInfo),
        };
    }
}

