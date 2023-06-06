// ================================================
//描 述 :  
//作 者 : 杜鑫 
//创建时间 : 2021-08-08 15-09-16  
//修改作者 : 杜鑫 
//修改时间 : 2021-08-08 15-09-16  
//版 本 : 0.1 
// ===============================================

using System;
using GameFramework;

public class MessengerInfo : IReference
{
    public object param1;
    public object param2;
    public object param3;
    public object param4;
    public object param5;
    public object param6;
    public object param7;
    public object param8;
    public object param9;
    public object param10;
    
    public Action action1;
    public Action action2;
    public Action action3;
    public Action<object> actionOneParam1;
    public Action<object> actionOneParam2;
    public Action<object> actionOneParam3;
    public Action<object,object> actionTwoParam1;
    public Action<object,object> actionTwoParam2;
    public Action<object,object> actionTwoParam3;

    /*
    MessengerInfo()
    {
    }
    MessengerInfo(object param1)
    {
        this.param1 = param1;
    }

    MessengerInfo(object param1, object param2)
    {
        this.param1 = param1;
        this.param2 = param2;
    }

    MessengerInfo(object param1, object param2, object param3)
    {
        this.param1 = param1;
        this.param2 = param2;
        this.param3 = param3;
    }

    MessengerInfo(object param1, object param2, object param3, object param4)
    {
        this.param1 = param1;
        this.param2 = param2;
        this.param3 = param3;
        this.param4 = param4;
    }

    MessengerInfo(object param1, object param2, object param3, object param4, object param5)
    {
        this.param1 = param1;
        this.param2 = param2;
        this.param3 = param3;
        this.param4 = param4;
        this.param5 = param5;
    }

    MessengerInfo(object param1, object param2, object param3, object param4, object param5, object param6)
    {
        this.param1 = param1;
        this.param2 = param2;
        this.param3 = param3;
        this.param6 = param6;
    }

    MessengerInfo(object param1, object param2, object param3, object param4, object param5, object param6,
        object param7)
    {
        this.param1 = param1;
        this.param2 = param2;
        this.param3 = param3;
        this.param4 = param4;
        this.param5 = param5;
        this.param6 = param6;
        this.param7 = param7;
    }

    MessengerInfo(object param1, object param2, object param3, object param4, object param5, object param6,
        object param7, object param8)
    {
        this.param1 = param1;
        this.param2 = param2;
        this.param3 = param3;
        this.param4 = param4;
        this.param5 = param5;
        this.param6 = param6;
        this.param7 = param7;
        this.param8 = param8;
    }

    MessengerInfo(object param1, object param2, object param3, object param4, object param5, object param6,
        object param7, object param8, object param9)
    {
        this.param1 = param1;
        this.param2 = param2;
        this.param3 = param3;
        this.param4 = param4;
        this.param5 = param5;
        this.param6 = param6;
        this.param7 = param7;
        this.param9 = param9;
    }

    MessengerInfo(object param1, object param2, object param3, object param4, object param5, object param6,
        object param7, object param8, object param9, object param10)
    {
        this.param1 = param1;
        this.param2 = param2;
        this.param3 = param3;
        this.param4 = param4;
        this.param5 = param5;
        this.param6 = param6;
        this.param7 = param7;
        this.param9 = param9;
        this.param10 = param10;
    }
    */

    public void Clear()
    {
        param1 = null;
        param2 = null;
        param3 = null;
        param4 = null;
        param5 = null;
        param6 = null;
        param7 = null;
        param8 = null;
        param9 = null;
        param10 = null;
        action1 = null;
        action2 = null;
        action3 = null;
        actionOneParam1 = null;
        actionOneParam2 = null;
        actionOneParam3 = null;
        actionTwoParam1 = null;
        actionTwoParam2 = null;
        actionTwoParam3 = null;
    }
}