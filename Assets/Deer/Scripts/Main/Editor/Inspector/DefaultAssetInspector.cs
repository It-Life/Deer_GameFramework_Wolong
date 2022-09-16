//====================================
//描 述 :  
//作 者 : 杜鑫 
//创建时间 : 2020/09/18 11:24:37  
//版 本 : 0.1 
// ===============================================
using UnityEngine;
using UnityEditor;
using System.IO;
using System;


[CanEditMultipleObjects, CustomEditor(typeof(DefaultAsset),false)]
public class DefaultAssetInspector : Editor
{
    private GUIStyle m_TextStyle;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (this.m_TextStyle == null)
        {
            this.m_TextStyle = "ScriptText";
        }
        bool enabled = GUI.enabled;
        GUI.enabled = true;
        string assetPath = AssetDatabase.GetAssetPath(target);
        if (assetPath.EndsWith(".lua") || assetPath.EndsWith(".properties") || assetPath.EndsWith(".gradle"))
        {
            string luaFile = File.ReadAllText(assetPath);
            string text;
            if (base.targets.Length > 1)
            {
                text = Path.GetFileName(assetPath);
            }
            else
            {
                text = luaFile;
                if (text.Length > 7000)
                {
                    text = text.Substring(0, 7000) + "...\n\n<...etc...>";
                }
            }
            Rect rect = GUILayoutUtility.GetRect(new GUIContent(text), this.m_TextStyle);
            rect.x = 0f;
            rect.y -= 3f;
            rect.width = EditorGUIUtility.currentViewWidth + 1f;
            GUI.Box(rect, text, this.m_TextStyle);
        }
        GUI.enabled = enabled;
    }
}