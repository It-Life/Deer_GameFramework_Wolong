using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GameObjectEdtior : MonoBehaviour
{
    [MenuItem("DeerTools/GameObjet/切换物体显隐状态 #A")]
    static void SetObjActive()
    {
        GameObject[] selectObjs = Selection.gameObjects;
        int objCtn = selectObjs.Length;
        for (int i = 0; i < objCtn; i++)
        {
            GameObject editingPrefabChild = selectObjs[i];
            bool isActive = selectObjs[i].activeSelf;
            editingPrefabChild.SetActive(!isActive);
            // 获取Prefab Asset 或连接该对象的Prefab Asset
            /*GameObject editingPrefabRoot = PrefabUtility.GetCorrespondingObjectFromSource(editingPrefabChild) as GameObject;

            // 确保我们已获取到一个Prefab
            if (editingPrefabRoot != null && PrefabUtility.GetPrefabAssetType(editingPrefabRoot) == PrefabAssetType.Regular)
            {
                // 打印预制件路径
                string prefabPath = AssetDatabase.GetAssetPath(editingPrefabRoot);
                // 保存预制件
                PrefabUtility.SaveAsPrefabAsset(editingPrefabRoot, prefabPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            else
            {
                Debug.Log("Selected object is not a Prefab Asset or a child object of a Prefab Asset.");
            }*/
        }  
    }
    

    [MenuItem("DeerTools/GameObjet/删除物体 %#d", false, 11)]
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
    [MenuItem("DeerTools/GameObjet/Apply GameObject #S")]
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
