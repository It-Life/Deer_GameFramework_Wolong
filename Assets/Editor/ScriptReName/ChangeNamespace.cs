// ================================================
//描 述:
//作 者:杜鑫
//创建时间:2022-06-19 19-21-15
//修改作者:杜鑫
//修改时间:2022-06-19 19-21-15
//版 本:0.1 
// ===============================================
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Please modify the description.
/// </summary>
public class ChangeNamespace : EditorWindow
{
    public static List<string> assetsPaths = new List<string>();
    public static string FolderPath;
    public static string TipContent;
    public static string NameSpace;
    private Vector2 scallPos;
    private bool isResult;
    private bool[] flags;
    private List<string> unAddPaths = new List<string>();

    //利用构造函数来设置窗口名称
    ChangeNamespace()
    {
        this.titleContent = new GUIContent("添加代码命名空间工具");
    }

    //添加菜单栏用于打开窗口
    [MenuItem("DeerTools/Script/添加代码命名空间工具")]
    static void showWindow()
    {
        assetsPaths.Clear();

        Rect windowRect = new Rect(0, 0, 600, 700);
        GetWindow(typeof(ChangeNamespace));
    }

    void OnGUI()
    {
        GUILayout.BeginVertical();

        GUILayout.Space(10);
        GUI.skin.label.fontSize = 14;
        //绘制文本
        GUILayout.BeginHorizontal();
        FolderPath = EditorGUILayout.TextField("Folder Path:", FolderPath, GUILayout.Height(16), GUILayout.Width(500));
        if (GUILayout.Button("···", GUILayout.Width(30), GUILayout.Height(16)))
        {
            FolderPath = UnityEditor.EditorUtility.SaveFolderPanel("要访问的文件夹路径", Application.dataPath + "/Scripts", "");
            GetFiles();
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        NameSpace = EditorGUILayout.TextField("NameSpace:", NameSpace, GUILayout.Width(300), GUILayout.Height(16));
        if (GUILayout.Button("添加命名空间", GUILayout.Width(100), GUILayout.Height(16)))
        {
            SetFiles();
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(20);
        GUI.skin.label.fontSize = 16;
        GUI.skin.label.alignment = TextAnchor.UpperLeft;
        GUILayout.Label("选择你要加入命名空间的代码文件:");

        GUILayout.Space(10);
        GUI.skin.label.fontSize = 14;
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("全选", GUILayout.Width(40), GUILayout.Height(16)))
        {
            SelectAll(true);
        }
        if (GUILayout.Button("取消", GUILayout.Width(40), GUILayout.Height(16)))
        {
            SelectAll(false);
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(10);
        scallPos = EditorGUILayout.BeginScrollView(scallPos, GUILayout.Height(500), GUILayout.Width(600));
        for (int i = 0; i < assetsPaths.Count; i++)
        {
            string lable = Path.GetFileName(assetsPaths[i]);
            flags[i] = EditorGUILayout.ToggleLeft(assetsPaths[i], flags[i], GUILayout.Width(600));
        }
        EditorGUILayout.EndScrollView();

        if (isResult)
        {
            //ResultPopup();
        }

        GUILayout.EndVertical();
    }
    void ShowPopup(int windowId)
    {
        if (string.IsNullOrEmpty(NameSpace))
            TipContent = "命名空间不能为空";
        else if (flags == null || !flags.ToList<bool>().Exists(s => s))
            TipContent = "未选择代码文件!";
        else if (flags.Length == unAddPaths.Count)
            TipContent = "没有代码文件添加成功！";
        else if (unAddPaths.Count <= 0)
            TipContent = "全部代码文件添加成功！";
        else
            TipContent = "部分代码文件添加成功！";
        //绘制标题
        GUILayout.Space(10);
        GUI.skin.label.fontSize = 24;
        GUI.skin.label.alignment = TextAnchor.MiddleCenter;
        GUILayout.Label(TipContent);

        if (unAddPaths.Count > 0)
        {
            GUILayout.Space(10);
            GUI.skin.label.fontSize = 16;
            GUI.skin.label.alignment = TextAnchor.UpperLeft;
            GUILayout.Label("未添加成功文件:");

            scallPos = EditorGUILayout.BeginScrollView(scallPos, GUILayout.Height(200), GUILayout.Width(600));
            for (int i = 0; i < unAddPaths.Count; i++)
            {
                string lable = Path.GetFileName(unAddPaths[i]);

                GUI.skin.label.fontSize = 14;
                GUILayout.Label(lable, GUILayout.Width(500), GUILayout.Height(16));
            }

            EditorGUILayout.EndScrollView();
        }

        if (GUILayout.Button("确定"))
        {
            isResult = false;
        }
    }
    void SelectAll(bool isAll)
    {
        if (flags == null || flags.Length <= 0)
            return;
        for (int i = 0; i < flags.Length; i++)
            flags[i] = isAll;
    }
    void GetFiles()
    {
        //获取指定路径下面的所有资源文件  
        if (Directory.Exists(FolderPath))
        {
            assetsPaths.Clear();
            DirectoryInfo direction = new DirectoryInfo(FolderPath);
            FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);

            Debug.Log(files.Length);

            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Name.EndsWith(".cs"))
                {
                    assetsPaths.Add(FolderPath + "/" + files[i].Name);
                }
            }
            flags = new bool[assetsPaths.Count];
        }
    }

    void SetFiles()
    {
        if (flags == null || string.IsNullOrEmpty(NameSpace))
        {
            isResult = true;
            return;
        }

        //string namespaceStr = "namespace ";
        string InsertStr = "namespace " + NameSpace;
        unAddPaths.Clear();
        for (int i = 0; flags != null && i < flags.Length; i++)
        {
            if (flags[i])
            {
                string content = File.ReadAllText(assetsPaths[i]);
                if (content.Contains(InsertStr))
                {
                    unAddPaths.Add(assetsPaths[i]);
                    continue;
                }
                content = InsertStr + "\n{\n" + content + "\n}";
                File.WriteAllText(assetsPaths[i], content);

            }
        }
        isResult = true;
    }
}