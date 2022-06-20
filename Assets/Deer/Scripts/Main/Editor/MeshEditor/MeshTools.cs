// ================================================
//描 述:
//作 者:XinDu
//创建时间:2022-05-26 14-46-42
//修改作者:XinDu
//修改时间:2022-05-26 14-46-42
//版 本:0.1 
// ===============================================
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Please modify the description.
/// </summary>
public class MeshTools
{
    [MenuItem("Assets/Mesh/SetOptimizeGameObjects")]
    public static void Optimize()
    {
        var fbxGo = Selection.activeGameObject;
        var fbxPath = AssetDatabase.GetAssetPath(fbxGo);
        var importer = AssetImporter.GetAtPath(fbxPath) as ModelImporter;
        if (importer == null)
        {
            return;
        }
        importer.optimizeGameObjects = true;
        importer.SaveAndReimport();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    [MenuItem("Assets/Mesh/UndoOptimizeGameObjects")]
    public static void UndoOptimize()
    {
        var fbxGo = Selection.activeGameObject;
        var fbxPath = AssetDatabase.GetAssetPath(fbxGo);//获取fbx在Project中的路径
        var importer = AssetImporter.GetAtPath(fbxPath) as ModelImporter;
        if (importer == null)
        {
            return;
        }
        importer.optimizeGameObjects = false;
        importer.SaveAndReimport();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    [MenuItem("Assets/Mesh/SearchSkinnedMeshRenderer")]
    public static void SearchSkinnedMeshRenderer()
    {
        var fbxGos = Selection.gameObjects;
        for (int i = 0; i < fbxGos.Length; i++)
        {
            SkinnedMeshRenderer[] skinnedMeshRenderers = fbxGos[i].GetComponentsInChildren<SkinnedMeshRenderer>();
            for (int j = 0; j < skinnedMeshRenderers.Length; j++)
            {
                if (skinnedMeshRenderers[j].skinnedMotionVectors)
                {
                    Debug.Log($"Prefab:{fbxGos[i].name} RootName:{skinnedMeshRenderers[j].gameObject.name} skinnedMotionVectors is True");
                }
            }
        }
    }
    [MenuItem("Assets/Mesh/SetSkinnedMeshRenderer")]
    public static void SetSkinnedMeshRenderer() 
    {
        var fbxGos = Selection.gameObjects;
        for (int i = 0; i < fbxGos.Length; i++)
        {
            SkinnedMeshRenderer[] skinnedMeshRenderers = fbxGos[i].GetComponentsInChildren<SkinnedMeshRenderer>();
            for (int j = 0; j < skinnedMeshRenderers.Length; j++)
            {
                if (skinnedMeshRenderers[j].skinnedMotionVectors)
                {
                    skinnedMeshRenderers[j].skinnedMotionVectors = false;
                }
            }
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

}