// ================================================
//描 述:
//作 者:杜鑫
//创建时间:2022-06-16 11-11-13
//修改作者:杜鑫
//修改时间:2022-06-16 11-11-13
//版 本:0.1 
// ===============================================

using UnityEditor;
using UnityEngine;
/// <summary>
/// Please modify the description.
/// </summary>
public class HuaTuoAOTGenerationWindow : EditorWindow
{
    [MenuItem("HuaTuo/AOT Generation")]
    public static HuaTuoAOTGenerationWindow GetWindow()
    {
        var window = GetWindow<HuaTuoAOTGenerationWindow>();
        window.titleContent = new GUIContent("HuaTuo AOT Generation");
        window.Focus();
        window.Repaint();
        return window;
    }
}