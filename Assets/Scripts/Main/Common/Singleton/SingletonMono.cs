/* ================================================
 * Introduction：xxx 
 * Creator：XinDu 
 * CreationTime：2022-03-25 16-39-57
 * ChangeCreator：XinDu 
 * ChangeTime：2022-03-25 16-39-57
 * CreateVersion：0.1
 *  =============================================== */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 不用手动创建拖动的MonoBehaviour 单例
/// 在第一次调用的时候会自动创建
/// 不建议使用，框架设置单例无对象化处理
/// </summary>
public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
{
    static T instance;
    public static T Instance 
    {
        get 
        {
            if (instance == null)
            {
                GameObject gameObject = new GameObject();
                gameObject.name = typeof(T).Name;
                instance = gameObject.AddComponent<T>();
                DontDestroyOnLoad(gameObject);
            }
            return instance; 
        }
    }
}