// ================================================
//描 述:
//作 者:AlanDu
//创建时间:2023-05-29 17-06-20
//修改作者:AlanDu
//修改时间:2023-05-29 17-06-20
//版 本:0.1 
// ===============================================
using UnityEngine;

/// <summary>
/// 调用安卓原生
/// </summary>
public partial class CrossPlatformManagerAndroid:ICrossPlatformManager
{
    private AndroidJavaObject GetCurrentActivity {
        get {
            AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            //AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.xxx.android.NoKillUnityPlayer");如果继承自UnityPlayer类则需要这一步
            return androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
        }
    }
    private bool ParamIsAvailable(object[] param) {
        return param != null && param.Length > 0;
    }
    private AndroidJavaObject CreateAndroidJavaObject(string className, object[] classParams) {
        AndroidJavaObject obj = null;
        if (ParamIsAvailable(classParams)) {
            obj = new AndroidJavaObject(className, classParams);
        } else {
            obj = new AndroidJavaObject(className);
        }
        return obj;
    }
    private void RunOnAndroidUIThread(System.Action action) {
        AndroidJavaObject activity = GetCurrentActivity;
        CallRunnable(activity, "runOnUiThread", false, action);
    }
    private void CallRunnable(AndroidJavaObject obj, string methodName, bool methodIsStatic, System.Action action) {
        if (action != null) {
            if(methodIsStatic) {
                obj.CallStatic(methodName, new AndroidJavaRunnable(() => {
                    action();
                }));
            } else {
                obj.Call(methodName, new AndroidJavaRunnable(() => {
                    action();
                }));
            }
        }
        obj.Dispose();
        
    }
    private void CallMethod(AndroidJavaObject obj, string methodName, object[] methodParams) {
        if (ParamIsAvailable(methodParams)) {
            obj.Call(methodName, methodParams);
        } else {
            obj.Call(methodName);
        }
        obj.Dispose();
    }
    private T CallStaticMethod<T>(AndroidJavaObject obj, string methodName, object[] methodParams) {
        T result = ParamIsAvailable(methodParams) ? obj.CallStatic<T>(methodName, methodParams) : obj.CallStatic<T>(methodName);
        obj.Dispose();
        return result;
    }
    private T CallMethod<T>(AndroidJavaObject obj, string methodName, object[] methodParams) {
        T result = ParamIsAvailable(methodParams) ? obj.Call<T>(methodName, methodParams) : obj.Call<T>(methodName);
        obj.Dispose();
        return result;
    }
    private void CallStaticMethod(AndroidJavaObject obj, string methodName, object methodParam)
    {
        if (methodParam == null)
        {
            obj.CallStatic(methodName);
        }
        else
        {
            obj.CallStatic(methodName, methodParam);
        }
        obj.Dispose();
    }
    private void CallStaticMethod(AndroidJavaObject obj, string methodName, object[] methodParams) {
        if (ParamIsAvailable(methodParams)) {
            obj.CallStatic(methodName, methodParams);
        } else {
            obj.CallStatic(methodName);
        }
        obj.Dispose();
    }
    /// <summary>
    /// 根据属性名称查找对象并调用其方法
    /// </summary>
    /// <typeparam name="T">返回值</typeparam>
    /// <param name="className">类名</param>
    /// <param name="fieldName">属性名</param>
    /// <param name="isFieldStatic">属性是否为静态</param>
    /// <param name="methodName">函数名称</param>
    /// <param name="methodParams">函数参数</param>
    /// <returns>返回值</returns>
    private T FindObject_CallMethod<T>(string className, string fieldName, bool isFieldStatic, string methodName, object[] methodParams) {
        AndroidJavaClass clazz = new AndroidJavaClass(className);
        AndroidJavaObject obj = isFieldStatic ? clazz.GetStatic<AndroidJavaObject>(fieldName) : clazz.Get<AndroidJavaObject>(fieldName);
        T result = ParamIsAvailable(methodParams) ? obj.Call<T>(methodName, methodParams) : obj.Call<T>(methodName);
        obj.Dispose();
        clazz.Dispose();  
        return result;
    }
    /// <summary>
    /// 根据属性名称查找对象并调用其方法
    /// </summary>
    /// <param name="className">类名</param>
    /// <param name="fieldName">属性名</param>
    /// <param name="isFieldStatic">属性是否为静态</param>
    /// <param name="methodName">函数名称</param>
    /// <param name="methodParams">函数参数</param>
    private void FindObject_CallMethod(string className, string fieldName, bool isFieldStatic, string methodName, object[] methodParams) {
        AndroidJavaClass clazz = new AndroidJavaClass(className);
        AndroidJavaObject obj = isFieldStatic ? clazz.GetStatic<AndroidJavaObject>(fieldName) : clazz.Get<AndroidJavaObject>(fieldName);
        obj.Call(methodName, methodParams);
        obj.Dispose();
        clazz.Dispose();
    }
    /// <summary>
    /// 根据属性名称查找对象并调用其静态方法
    /// </summary>
    /// <typeparam name="T">返回值</typeparam>
    /// <param name="className">类名</param>
    /// <param name="fieldName">属性名</param>
    /// <param name="isFieldStatic">属性是否为静态</param>
    /// <param name="methodName">函数名称</param>
    /// <param name="methodParams">函数参数</param>
    /// <returns>返回值</returns>
    private T FindObject_CallStaticMethod<T>(string className, string fieldName, bool isFieldStatic, string methodName, object[] methodParams) {
        AndroidJavaClass clazz = new AndroidJavaClass(className);
        AndroidJavaObject obj = isFieldStatic ? clazz.GetStatic<AndroidJavaObject>(fieldName) : clazz.Get<AndroidJavaObject>(fieldName);
        T result = ParamIsAvailable(methodParams) ? obj.CallStatic<T>(methodName, methodParams) : clazz.CallStatic<T>(methodName);
        obj.Dispose();
        clazz.Dispose();
        return result;
    }
    /// <summary>
    /// 根据属性名称查找对象并调用其静态方法
    /// </summary>
    /// <param name="className">类名</param>
    /// <param name="fieldName">属性名</param>
    /// <param name="isFieldStatic">属性是否为静态</param>
    /// <param name="methodName">函数名称</param>
    /// <param name="methodParams">函数参数</param>
    private void FindObject_CallStaticMethod(string className, string fieldName, bool isFieldStatic, string methodName, object[] methodParams) {
        AndroidJavaClass clazz = new AndroidJavaClass(className);
        AndroidJavaObject obj = isFieldStatic ? clazz.GetStatic<AndroidJavaObject>(fieldName) : clazz.Get<AndroidJavaObject>(fieldName);
        obj.CallStatic(methodName, methodParams);
        obj.Dispose();
        clazz.Dispose();
    }
    /// <summary>
    /// 创建对象并调用其方法
    /// </summary>
    /// <typeparam name="T">返回值</typeparam>
    /// <param name="className">类名称</param>
    /// <param name="classParams">构造类参数</param>
    /// <param name="methodName">函数名称</param>
    /// <param name="methodParams">函数参数</param>
    /// <returns>返回值</returns>
    private T CreateObject_CallMethod<T>(string className, object[] classParams, string methodName, object[] methodParams) {
        AndroidJavaObject obj = CreateAndroidJavaObject(className, classParams);
        return CallMethod<T>(obj, methodName, methodParams);
    }
    /// <summary>
    /// 创建对象并调用其方法
    /// </summary>
    /// <param name="className">类名称</param>
    /// <param name="classParams">构造类参数</param>
    /// <param name="methodName">函数名称</param>
    /// <param name="methodParams">函数参数</param>
    private void CreateObject_CallMethod(string className, object[] classParams, string methodName, object[] methodParams) {
        AndroidJavaObject obj = CreateAndroidJavaObject(className, classParams);
        CallMethod(obj, methodName, methodParams);
    }
    /// <summary>
    /// 创建对象并调用其静态方法
    /// </summary>
    /// <typeparam name="T">返回值</typeparam>
    /// <param name="className">类名称</param>
    /// <param name="classParams">构造类参数</param>
    /// <param name="methodName">函数名称</param>
    /// <param name="methodParams">函数参数</param>
    /// <returns>返回值</returns>
    private T CreateObject_CallStaticMethod<T>(string className, object[] classParams, string methodName, object[] methodParams) {
        AndroidJavaObject obj = CreateAndroidJavaObject(className, classParams);
        return CallStaticMethod<T>(obj, methodName, methodParams);
    }
    /// <summary>
    /// 创建对象并调用其静态方法
    /// </summary>
    /// <param name="className">类名称</param>
    /// <param name="classParams">构造类参数</param>
    /// <param name="methodName">函数名称</param>
    /// <param name="methodParams">函数参数</param>
    private void CreateObject_CallStaticMethod(string className, object[] classParams, string methodName, object[] methodParams) {
        AndroidJavaObject obj = CreateAndroidJavaObject(className, classParams);
        CallStaticMethod(obj, methodName, methodParams);
    }
    private void CreateObject_CallStaticMethod(string className, object[] classParams, string methodName, object methodParam)
    {
        AndroidJavaObject obj = CreateAndroidJavaObject(className, classParams);
        CallStaticMethod(obj, methodName, methodParam);
    }
    private void CreateClass_CallStaticMethod(string className,string methodName,params object[] methodParams)
    {
        AndroidJavaClass androidClass = new AndroidJavaClass(className);
        if (methodParams==null || methodParams.Length<=0)
        {
            androidClass.CallStatic(methodName);
        }else
        {
            androidClass.CallStatic(methodName, methodParams);
        }
    }
    private T CreateClass_CallStaticMethod<T>(string className,string methodName,params object[] methodParams)
    {
        AndroidJavaClass androidClass = new AndroidJavaClass(className);
        if (methodParams==null || methodParams.Length<=0)
        {
            return androidClass.CallStatic<T>(methodName);
        }
        return androidClass.CallStatic<T>(methodName, methodParams);
    }

    private T GetField<T>(string className, string fieldName, bool isFieldStatic) {
        AndroidJavaClass clazz = new AndroidJavaClass(className);
        return isFieldStatic ? clazz.GetStatic<T>(fieldName) : clazz.Get<T>(fieldName);
    }
}
