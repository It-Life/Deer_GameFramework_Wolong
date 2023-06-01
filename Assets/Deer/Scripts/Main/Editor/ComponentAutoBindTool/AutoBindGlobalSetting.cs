using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

[Serializable]
public class AutoBindRulePrefixe
{
    public string Prefixe;
    public string FullName;

    public AutoBindRulePrefixe(string prefixe,string fullName)
    {
        Prefixe = prefixe;
        FullName = fullName;
    }
}

/// <summary>
/// 自动绑定全局设置
/// </summary>
[CreateAssetMenu(fileName = "AutoBindGlobalSetting", menuName = "Deer/Auto Bind Setting", order = 41)]
public class AutoBindGlobalSetting : ScriptableObject
{
    [SerializeField]
    private string m_Namespace;
    [SerializeField]
    //[FolderPath]
    //[LabelText("默认组件代码保存路径")]
    private string m_ComCodePath;
    [SerializeField]
    //[FolderPath]
    //[LabelText("默认挂载代码保存路径")]
    private string m_MountCodePath;
    
    [SerializeField] [LabelText("默认挂载代码搜寻程序集")]
    private List<string> m_MountScriptListAssemblys;

    [SerializeField] [LabelText("组件的缩略名字映射")]
    private List<AutoBindRulePrefixe> m_RulePrefixes= new List<AutoBindRulePrefixe>()
    {
        new AutoBindRulePrefixe("Trans","Transform"),
        new AutoBindRulePrefixe("OldAnim","Animation"),
        new AutoBindRulePrefixe("NewAnim","Animator"),
        new AutoBindRulePrefixe("Rect","RectTransform"),
        new AutoBindRulePrefixe("Canvas","Canvas"),
        new AutoBindRulePrefixe("Group","CanvasGroup"),
        new AutoBindRulePrefixe("VGroup","VerticalLayoutGroup"),
        new AutoBindRulePrefixe("HGroup","HorizontalLayoutGroup"),
        new AutoBindRulePrefixe("GGroup","GridLayoutGroup"),
        new AutoBindRulePrefixe("TGroup","ToggleGroup"),
        new AutoBindRulePrefixe("Btn","UIButtonSuper"),
        new AutoBindRulePrefixe("BtnP","ButtonPro"),
        new AutoBindRulePrefixe("Img","Image"),
        new AutoBindRulePrefixe("RImg","RawImage"),
        new AutoBindRulePrefixe("Txt","Text"),
        new AutoBindRulePrefixe("TxtM","TextMeshProUGUI"),
        new AutoBindRulePrefixe("Input","TMP_InputField"),
        new AutoBindRulePrefixe("Slider","Slider"),
        new AutoBindRulePrefixe("Mask","Mask"),
        new AutoBindRulePrefixe("Mask2D","RectMask2D"),
        new AutoBindRulePrefixe("Tog","Toggle"),
        new AutoBindRulePrefixe("Sbar","Scrollbar"),
        new AutoBindRulePrefixe("SRect","ScrollRect"),
        new AutoBindRulePrefixe("Drop","Dropdown"),
        new AutoBindRulePrefixe("USpriteAni","UGUISpriteAnimation"),
        new AutoBindRulePrefixe("VGridV","LoopGridView"),
        new AutoBindRulePrefixe("HGridV","LoopGridView"),
        new AutoBindRulePrefixe("VListV","LoopListView2"),
        new AutoBindRulePrefixe("HListV","LoopListView2"),
        new AutoBindRulePrefixe("Map","RadarMap"),
    };

    public List<AutoBindRulePrefixe> RulePrefixes
    {
        get { return m_RulePrefixes; }
    }

    public string Namespace
    {
        get
        {
            return m_Namespace;
        }
    }
    public string MountCodePath
    {
        get
        {
            return m_MountCodePath;
        }

    }

    public string ComCodePath
    {
        get
        {
            return m_ComCodePath;
        }
    }
    
    public List<string> MountScriptListAssemblys
    {
        get
        {
            return m_MountScriptListAssemblys;
        }
    }
    
    public static bool IsValidBind( Transform target, List<string> filedNames, List<string> componentTypeNames)
    {
        string[] strArray = target.name.Split('_');

        if (strArray.Length == 1)
        {
            return false;
        }

        bool isFind = false;
        string filedName = strArray[^1];
        for (int i = 0; i < strArray.Length - 1; i++)
        {
            string str = strArray[i].Replace("#","");
            string comName;
            var _AutoBindGlobalSetting = GetAutoBindGlobalSetting();
            var _PrefixesDict = _AutoBindGlobalSetting.RulePrefixes;
            bool isFindComponent = false;
            foreach (var autoBindRulePrefix in _PrefixesDict)
            {
                if (autoBindRulePrefix.Prefixe.ToLower().Equals(str.ToLower()))
                {
                    comName = autoBindRulePrefix.FullName;
                    str = char.ToUpper(str[0]) + str.Substring(1);
                    filedNames.Add($"{str}_{filedName}");
                    componentTypeNames.Add(comName);
                    isFind = true;
                    isFindComponent = true;
                    break;
                }
            }
            if (!isFindComponent)
            {
                Debug.LogWarning($"{target.name}的命名中{str}不存在对应的组件类型，绑定失败");
            }
        }
        if (!isFind)
        {
            return false;
        }
        return true;
    }
    
    private static void CreateAutoBindGlobalSetting()
    {
        string[] paths = AssetDatabase.FindAssets("t:AutoBindGlobalSetting");
        if (paths.Length >= 1)
        {
            string path = AssetDatabase.GUIDToAssetPath(paths[0]);
            EditorUtility.DisplayDialog("警告", $"已存在AutoBindGlobalSetting，路径:{path}", "确认");
            return;
        }
        AutoBindGlobalSetting setting = CreateInstance<AutoBindGlobalSetting>();
        AssetDatabase.CreateAsset(setting, "Assets/AutoBindGlobalSetting.asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public static AutoBindGlobalSetting GetAutoBindGlobalSetting()
    {
        AutoBindGlobalSetting _AutoBindGlobalSetting = null;
        string[] paths = AssetDatabase.FindAssets("t:AutoBindGlobalSetting");
        if (paths.Length == 0)
        {
            Debug.LogError("不存在AutoBindGlobalSetting");
            return _AutoBindGlobalSetting;
        }
        if (paths.Length > 1)
        {
            Debug.LogError("AutoBindGlobalSetting数量大于1");
            return _AutoBindGlobalSetting;
        }
        string path = AssetDatabase.GUIDToAssetPath(paths[0]);
        _AutoBindGlobalSetting = AssetDatabase.LoadAssetAtPath<AutoBindGlobalSetting>(path);
        return _AutoBindGlobalSetting;
    }
}
