using UnityEngine;
using UnityEditor;

/// <summary>
/// 
/// author:罐子（Lawliet）
/// vindicator:对比的菜单
/// versions:0.0.1
/// introduce:主要对两个GameObject或者两个Component进行对比，并返回对应的差异树
/// note:
/// 
/// 
/// list:
/// 
/// 
/// 
/// </summary>
namespace UnityCompare
{
    public class CompareEditor : ScriptableObject
    {
        [MenuItem("Assets/Compare")]
        static void Compare()
        {
            var gameObjects = Selection.gameObjects;

            if (gameObjects.Length != 2)
            {
                EditorUtility.DisplayDialog("Error", "需要选中两个Prefab进行对比", "ok");
            }
            else
            {
                var left = gameObjects[0];
                var right = gameObjects[1];

                CompareWindow.ComparePrefab(left, right);
            }
        }
    }
}