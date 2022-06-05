// ================================================
//描 述 :  
//作 者 : 杜鑫 
//创建时间 : 2021-07-11 22-36-22  
//修改作者 : 杜鑫 
//修改时间 : 2021-07-11 22-36-22  
//版 本 : 0.1 
// ===============================================
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
namespace Deer
{
    public class AttributeCallBack
    {
        [OnOpenAssetAttribute(1)]
        public static bool SingleSelect(int instanceID, int line)
        {
/*            Object obj = EditorUtility.InstanceIDToObject(instanceID);
            string name = obj.name;
            Debug.Log("Open Asset Step1,asset name=>" + name);*/
            return false;
        }
        [OnOpenAssetAttribute(2)]
        public static bool DoubleSelect(int instanceID, int line)
        {
            UnityEngine.Object obj = EditorUtility.InstanceIDToObject(instanceID);
            string path = AssetDatabase.GetAssetPath(obj);
            //文件路径
            string pathName = Application.dataPath + "/" + path.Replace("Assets/", "");
            //指定打开文件类型
            if (pathName.EndsWith(".lua"))
            {
                //IDEUtils.OpenFileWith(pathName);
                //string name = obj.name;
                //Debug.Log("DoubleSelect,asset asset name=>" + name);
                return true;
            }
            return false;
        }
    }
}