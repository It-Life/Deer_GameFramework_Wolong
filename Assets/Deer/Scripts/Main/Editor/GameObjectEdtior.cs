using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GameObjectEdtior : MonoBehaviour
{
    [MenuItem("MyTools/Lazy/切换物体显隐状态 #A")]
    static void SetObjActive()
    {
        GameObject[] selectObjs = Selection.gameObjects;
        int objCtn = selectObjs.Length;
        for (int i = 0; i < objCtn; i++)
        {
            bool isAcitve = selectObjs[i].activeSelf;
            selectObjs[i].SetActive(!isAcitve);
        }  
    }
    

    [MenuItem("MyTools/Lazy/删除物体 %#d", false, 11)]
    static void MyDeleteObject()
    {
        foreach (Object o in Selection.objects)
        {
            //GameObject.DestroyImmediate(o);
            Undo.DestroyObjectImmediate(o);//利用Undo进行的删除操作 是可以撤销的
        }
        //需要把删除操作注册到 操作记录里面
    }
    
    //快捷键控制保存Prefab Shift + S
    [MenuItem("MyTools/Lazy/Apply GameObject #S")]
    [System.Obsolete]
    public static void ApplyPrefab()
    {
        GameObject go = Selection.activeGameObject;
        if (go == null) return;
        PrefabType type = PrefabUtility.GetPrefabType(go);
        if (type  == PrefabType.PrefabInstance)
        {
            Object target = PrefabUtility.GetCorrespondingObjectFromSource(go);
            PrefabUtility.ReplacePrefab(go, target, ReplacePrefabOptions.ConnectToPrefab);
        }

    }
}
