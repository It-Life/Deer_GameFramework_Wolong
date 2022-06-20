// ================================================
//描 述 :  
//作 者 : 杜鑫 
//创建时间 : 2022-03-04 19-39-47  
//修改作者 : 杜鑫 
//修改时间 : 2022-03-04 19-39-47  
//版 本 : 0.1 
// ===============================================
using UnityEngine;
using UnityEditor;
using System.IO;

[CanEditMultipleObjects, CustomEditor(typeof(TextAsset))]
public class TextAssetInspector : Editor
{
    private GUIStyle m_TextStyle;

    public override void OnInspectorGUI()
    {
        if (this.m_TextStyle == null)
        {
            this.m_TextStyle = "ScriptText";
        }
        bool enabled = GUI.enabled;
        GUI.enabled = true;
        string assetPath = AssetDatabase.GetAssetPath(target);
        if (assetPath.EndsWith(".md") 
            || assetPath.EndsWith(".xml") 
            || assetPath.EndsWith(".txt") 
            || assetPath.EndsWith(".html")
            || assetPath.EndsWith(".csv"))
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