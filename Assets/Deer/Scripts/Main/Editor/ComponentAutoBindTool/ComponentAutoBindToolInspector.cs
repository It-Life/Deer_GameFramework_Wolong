using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using BindData = ComponentAutoBindTool.BindData;
using System.Reflection;
using System.IO;
using UnityEditor.Callbacks;

[CustomEditor(typeof(ComponentAutoBindTool))]
public class ComponentAutoBindToolInspector : Editor
{

    private ComponentAutoBindTool m_Target;
    private static ComponentAutoBindTool m_target;

    private SerializedProperty m_BindDatas;
    private SerializedProperty m_BindComs;
    private List<BindData> m_TempList = new List<BindData>();
    private List<string> m_TempFiledNames = new List<string>();
    private List<string> m_TempComponentTypeNames = new List<string>();

    private string[] s_AssemblyNames = { "Main.Runtime" };
    private string[] m_HelperTypeNames;
    private string m_HelperTypeName;
    private int m_HelperTypeNameIndex;

    private AutoBindGlobalSetting m_Setting;

    private SerializedProperty m_Namespace;
    private SerializedProperty m_ClassName;
    private SerializedProperty m_CodePath;
    private SerializedProperty m_CodeRelativePath;

    private void OnEnable()
    {
        m_Target = (ComponentAutoBindTool)target;
        m_target = (ComponentAutoBindTool)target;
        m_BindDatas = serializedObject.FindProperty("BindDatas");
        m_BindComs = serializedObject.FindProperty("m_BindComs");

        m_HelperTypeNames = GetTypeNames(typeof(IAutoBindRuleHelper), s_AssemblyNames);

        string[] paths = AssetDatabase.FindAssets("t:AutoBindGlobalSetting");
        if (paths.Length == 0)
        {
            Debug.LogError("不存在AutoBindGlobalSetting");
            return;
        }
        if (paths.Length > 1)
        {
            Debug.LogError("AutoBindGlobalSetting数量大于1");
            return;
        }
        string path = AssetDatabase.GUIDToAssetPath(paths[0]);
        m_Setting = AssetDatabase.LoadAssetAtPath<AutoBindGlobalSetting>(path);


        m_Namespace = serializedObject.FindProperty("m_Namespace");
        m_ClassName = serializedObject.FindProperty("m_ClassName");
        m_CodePath = serializedObject.FindProperty("m_CodePath");
        m_CodeRelativePath = serializedObject.FindProperty("m_CodeRelativePath");

        m_Namespace.stringValue = string.IsNullOrEmpty(m_Namespace.stringValue) ? m_Setting.Namespace : m_Namespace.stringValue;
        m_ClassName.stringValue = string.IsNullOrEmpty(m_ClassName.stringValue) ? m_Target.gameObject.name : m_ClassName.stringValue;
        m_CodeRelativePath.stringValue = string.IsNullOrEmpty(m_CodeRelativePath.stringValue) ? m_Setting.CodeRelativePath : m_CodeRelativePath.stringValue;
        m_CodePath.stringValue = Application.dataPath + m_CodeRelativePath.stringValue;

        serializedObject.ApplyModifiedProperties();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawTopButton();

        DrawHelperSelect();

        DrawSetting();

        DrawKvData();

        serializedObject.ApplyModifiedProperties();


    }

    /// <summary>
    /// 绘制顶部按钮
    /// </summary>
    private void DrawTopButton()
    {
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("排序"))
        {
            Sort();
        }

        if (GUILayout.Button("全部删除"))
        {
            RemoveAll();
        }

        if (GUILayout.Button("删除空引用"))
        {
            RemoveNull();
        }

        if (GUILayout.Button("自动绑定组件"))
        {
            AutoBindComponent();
        }

        if (GUILayout.Button("生成绑定代码"))
        {
            GenAutoBindCode();
        }

        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// 排序
    /// </summary>
    private void Sort()
    {


        m_TempList.Clear();
        foreach (BindData data in m_Target.BindDatas)
        {
            m_TempList.Add(new BindData(data.Name, data.BindCom));
        }
        m_TempList.Sort((x, y) =>
        {
            return string.Compare(x.Name, y.Name, StringComparison.Ordinal);
        });

        m_BindDatas.ClearArray();
        foreach (BindData data in m_TempList)
        {
            AddBindData(data.Name, data.BindCom);
        }

        SyncBindComs();
    }

    /// <summary>
    /// 全部删除
    /// </summary>
    private void RemoveAll()
    {
        m_BindDatas.ClearArray();

        SyncBindComs();
    }

    /// <summary>
    /// 删除空引用
    /// </summary>
    private void RemoveNull()
    {
        for (int i = m_BindDatas.arraySize - 1; i >= 0; i--)
        {
            SerializedProperty element = m_BindDatas.GetArrayElementAtIndex(i).FindPropertyRelative("BindCom");
            if (element.objectReferenceValue == null)
            {
                m_BindDatas.DeleteArrayElementAtIndex(i);
            }
        }

        SyncBindComs();
    }

    /// <summary>
    /// 自动绑定组件
    /// </summary>
    private void AutoBindComponent()
    {
        m_BindDatas.ClearArray();

        Transform[] childs = m_Target.gameObject.GetComponentsInChildren<Transform>(true);
        foreach (Transform child in childs)
        {
            m_TempFiledNames.Clear();
            m_TempComponentTypeNames.Clear();

            if (m_Target.RuleHelper.IsValidBind(child, m_TempFiledNames, m_TempComponentTypeNames))
            {
                for (int i = 0; i < m_TempFiledNames.Count; i++)
                {
                    Component com = child.GetComponent(m_TempComponentTypeNames[i]);
                    if (com == null)
                    {
                        Debug.LogError($"{child.name}上不存在{m_TempComponentTypeNames[i]}的组件");
                    }
                    else
                    {
                        AddBindData(m_TempFiledNames[i], child.GetComponent(m_TempComponentTypeNames[i]));
                    }

                }
            }
        }

        SyncBindComs();
    }

    /// <summary>
    /// 绘制辅助器选择框
    /// </summary>
    private void DrawHelperSelect()
    {
        m_HelperTypeName = m_HelperTypeNames[0];

        if (m_Target.RuleHelper != null)
        {
            m_HelperTypeName = m_Target.RuleHelper.GetType().Name;

            for (int i = 0; i < m_HelperTypeNames.Length; i++)
            {
                if (m_HelperTypeName == m_HelperTypeNames[i])
                {
                    m_HelperTypeNameIndex = i;
                }
            }
        }
        else
        {
            IAutoBindRuleHelper helper = (IAutoBindRuleHelper)CreateHelperInstance(m_HelperTypeName, s_AssemblyNames);
            m_Target.RuleHelper = helper;
        }

        foreach (GameObject go in Selection.gameObjects)
        {
            ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();
            if (autoBindTool.RuleHelper == null)
            {
                IAutoBindRuleHelper helper = (IAutoBindRuleHelper)CreateHelperInstance(m_HelperTypeName, s_AssemblyNames);
                autoBindTool.RuleHelper = helper;
            }
        }

        int selectedIndex = EditorGUILayout.Popup("AutoBindRuleHelper", m_HelperTypeNameIndex, m_HelperTypeNames);
        if (selectedIndex != m_HelperTypeNameIndex)
        {
            m_HelperTypeNameIndex = selectedIndex;
            m_HelperTypeName = m_HelperTypeNames[selectedIndex];
            IAutoBindRuleHelper helper = (IAutoBindRuleHelper)CreateHelperInstance(m_HelperTypeName, s_AssemblyNames);
            m_Target.RuleHelper = helper;

        }
    }

    /// <summary>
    /// 绘制设置项
    /// </summary>
    private void DrawSetting()
    {
        EditorGUILayout.BeginHorizontal();
        m_Namespace.stringValue = EditorGUILayout.TextField(new GUIContent("命名空间："), m_Namespace.stringValue);
        if (GUILayout.Button("默认设置"))
        {
            m_Namespace.stringValue = m_Setting.Namespace;
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        m_ClassName.stringValue = EditorGUILayout.TextField(new GUIContent("类名："), m_ClassName.stringValue);
        if (GUILayout.Button("物体名"))
        {
            m_ClassName.stringValue = m_Target.gameObject.name;
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField("代码保存路径：");
        EditorGUILayout.LabelField(m_CodePath.stringValue);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("选择路径"))
        {
            string temp = m_CodePath.stringValue;
            string path = EditorUtility.OpenFolderPanel("选择代码保存路径", Application.dataPath, "");
            m_CodeRelativePath.stringValue = path.Replace(Application.dataPath, "");
            m_CodePath.stringValue = path;
            if (string.IsNullOrEmpty(m_CodePath.stringValue))
            {
                m_CodePath.stringValue = temp;
            }
        }
        if (GUILayout.Button("默认设置"))
        {
            m_CodePath.stringValue = m_Setting.CodePath;
        }
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// 绘制键值对数据
    /// </summary>
    private void DrawKvData()
    {
        //绘制key value数据

        int needDeleteIndex = -1;

        EditorGUILayout.BeginVertical();
        SerializedProperty property;

        for (int i = 0; i < m_BindDatas.arraySize; i++)
        {

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"[{i}]", GUILayout.Width(25));
            property = m_BindDatas.GetArrayElementAtIndex(i).FindPropertyRelative("Name");
            property.stringValue = EditorGUILayout.TextField(property.stringValue, GUILayout.Width(150));
            property = m_BindDatas.GetArrayElementAtIndex(i).FindPropertyRelative("BindCom");
            property.objectReferenceValue = EditorGUILayout.ObjectField(property.objectReferenceValue, typeof(Component), true);

            if (GUILayout.Button("X"))
            {
                //将元素下标添加进删除list
                needDeleteIndex = i;
            }
            EditorGUILayout.EndHorizontal();
        }

        //删除data
        if (needDeleteIndex != -1)
        {
            m_BindDatas.DeleteArrayElementAtIndex(needDeleteIndex);
            SyncBindComs();
        }

        EditorGUILayout.EndVertical();
    }



    /// <summary>
    /// 添加绑定数据
    /// </summary>
    private void AddBindData(string name, Component bindCom)
    {
        int index = m_BindDatas.arraySize;
        m_BindDatas.InsertArrayElementAtIndex(index);
        SerializedProperty element = m_BindDatas.GetArrayElementAtIndex(index);
        element.FindPropertyRelative("Name").stringValue = name;
        element.FindPropertyRelative("BindCom").objectReferenceValue = bindCom;

    }

    /// <summary>
    /// 同步绑定数据
    /// </summary>
    private void SyncBindComs()
    {
        m_BindComs.ClearArray();

        for (int i = 0; i < m_BindDatas.arraySize; i++)
        {
            SerializedProperty property = m_BindDatas.GetArrayElementAtIndex(i).FindPropertyRelative("BindCom");
            m_BindComs.InsertArrayElementAtIndex(i);
            m_BindComs.GetArrayElementAtIndex(i).objectReferenceValue = property.objectReferenceValue;
        }
    }

    /// <summary>
    /// 获取指定基类在指定程序集中的所有子类名称
    /// </summary>
    private string[] GetTypeNames(Type typeBase, string[] assemblyNames)
    {
        List<string> typeNames = new List<string>();
        foreach (string assemblyName in assemblyNames)
        {
            Assembly assembly = null;
            try
            {
                assembly = Assembly.Load(assemblyName);
            }
            catch
            {
                continue;
            }

            if (assembly == null)
            {
                continue;
            }

            Type[] types = assembly.GetTypes();
            foreach (Type type in types)
            {
                if (type.IsClass && !type.IsAbstract && typeBase.IsAssignableFrom(type))
                {
                    typeNames.Add(type.FullName);
                }
            }
        }

        typeNames.Sort();
        return typeNames.ToArray();
    }

    /// <summary>
    /// 创建辅助器实例
    /// </summary>
    private object CreateHelperInstance(string helperTypeName, string[] assemblyNames)
    {
        foreach (string assemblyName in assemblyNames)
        {
            Assembly assembly = Assembly.Load(assemblyName);

            object instance = assembly.CreateInstance(helperTypeName);
            if (instance != null)
            {
                return instance;
            }
        }

        return null;
    }

    /// <summary>
    /// 写入第三方引用
    /// </summary>
    /// <param name="streamWriter">写入流</param>
    private void WriteUsing(StreamWriter streamWriter) 
    {
        //根据索引获取
        for (int i = 0; i < m_Target.BindDatas.Count; i++)
        {
            BindData data = m_Target.BindDatas[i];
            if (data.BindCom.GetType().Name == "TextMeshProUGUI")
            {
                streamWriter.WriteLine("using TMPro;");
            }
        }
    }

    /// <summary>
    /// 生成自动绑定代码
    /// </summary>
    private void GenAutoBindCode()
    {
        GameObject go = m_Target.gameObject;

        string className = !string.IsNullOrEmpty(m_Target.ClassName) ? m_Target.ClassName : go.name;
        string codePath = !string.IsNullOrEmpty(m_Target.CodePath) ? m_Target.CodePath : m_Setting.CodePath;

        if (!Directory.Exists(codePath))
        {
            Debug.LogError($"{go.name}的代码保存路径{codePath}无效");
        }


        using (StreamWriter sw = new StreamWriter($"{codePath}/{className}.BindComponents.cs"))
        {
            //sw.WriteLine("using System.Collections;");
            //sw.WriteLine("using System.Collections.Generic;");
            WriteUsing(sw);
            sw.WriteLine("using UnityEngine;");
            sw.WriteLine("using UnityEngine.UI;");

            if (!string.IsNullOrEmpty(m_Target.Namespace))
                sw.WriteLine($"\nnamespace {m_Target.Namespace}" + "\n{");
            else
                sw.WriteLine("\nnamespace PleaseAmendNamespace\n{");

            //sw.WriteLine($"\t/*\n\t* Introduction：\n\t* Creator：xxxx\n\t* CreationTime：{DateTime.Now}\n\t*/");
            sw.WriteLine("\tpublic partial class " + className + "\n\t{");

            //组件字段
            foreach (BindData data in m_Target.BindDatas)
            {
                sw.WriteLine($"\t\tprivate {data.BindCom.GetType().Name} m_{data.Name};");
            }

            sw.WriteLine("\n\t\tprivate void GetBindComponents(GameObject go)\n\t\t{");

            //获取autoBindTool上的Component
            sw.WriteLine($"\t\t\tComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();\n");

            //根据索引获取
            for (int i = 0; i < m_Target.BindDatas.Count; i++)
            {
                BindData data = m_Target.BindDatas[i];
                string filedName = $"m_{data.Name}";
                sw.WriteLine($"\t\t\t{filedName} = autoBindTool.GetBindComponent<{data.BindCom.GetType().Name}>({i});");
            }

            sw.WriteLine("\t\t}\n\t}\n}");
            sw.Close();
        }

        //GenAutoBindMountCode(go, className, codePath);
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("提示", "代码生成完毕,正在挂载", "OK");
    }
    /// <summary>
    /// 生成自动绑定的挂载代码
    /// </summary>
    private void GenAutoBindMountCode(GameObject go, string className, string codePath)
    {
        using (StreamWriter sw = new StreamWriter($"{codePath}/{className}.cs"))
        {
            sw.WriteLine("/* ================================================");
            sw.WriteLine($" * Introduction：xxxx\n * Creator：xxxx\n * CreationTime：{DateTime.Now}\n * CreateVersion：{DeerSettingsUtils.FrameworkGlobalSettings.ScriptVersion}");
            sw.WriteLine("================================================ */");

            sw.WriteLine("using System.Collections;");
            sw.WriteLine("using System.Collections.Generic;");
            sw.WriteLine("using UnityEngine;");
            //sw.WriteLine("using UnityEngine.UI;");

            if (!string.IsNullOrEmpty(m_Target.Namespace))
                sw.WriteLine($"\nnamespace {m_Target.Namespace}" + "\n{");
            else
                sw.WriteLine("\nnamespace PleaseAmendNamespace\n{");

            sw.WriteLine($"\t/// <summary>\n\t/// Please modify the description.\n\t/// </summary>");
            sw.WriteLine("\tpublic partial class " + className + " : UIBaseForms\n\t{");

            #region Start
            sw.WriteLine("\t\tvoid Start() {\n\t\t\tGetBindComponents(gameObject);\n");         //   Start
            for (int i = 0; i < m_Target.BindDatas.Count; i++)
            {
                if(Equals(m_Target.BindDatas[i].BindCom.GetType(), typeof(UIButtonSuper)))
                    sw.WriteLine($"\t\t\tm_{m_Target.BindDatas[i].Name}.onClick.AddListener({m_Target.BindDatas[i].Name}Event);");
            }
            sw.WriteLine("\t\t}\n");
            #endregion

            #region OnDisable
            sw.WriteLine("\t\tvoid OnDisable() {");     //   OnDisable
            for (int i = 0; i < m_Target.BindDatas.Count; i++)
            {
                if (Equals(m_Target.BindDatas[i].BindCom.GetType(), typeof(UIButtonSuper)))
                    sw.WriteLine($"\t\t\tm_{m_Target.BindDatas[i].Name}.onClick.RemoveAllListeners();");
            }
            sw.WriteLine("\t\t}\n");
            #endregion

            #region ButtonEvent
            for (int i = 0; i < m_Target.BindDatas.Count; i++)
            {
                if (Equals(m_Target.BindDatas[i].BindCom.GetType(), typeof(UIButtonSuper)))
                {
                    sw.WriteLine("\t\tprivate void " + m_Target.BindDatas[i].Name + "Event() {");
                    sw.WriteLine("\t\t\tDebug.Log(" + @$"""This is {m_Target.BindDatas[i].Name}Event.""" + ");\n\t\t}\n");
                }
            }
            #endregion


            sw.WriteLine("\t}\n}");
            sw.Close();
        }

        EditorPrefs.SetString("DEFAULT-CLASS-NAME", m_Target.ClassName);
        EditorPrefs.SetBool("DEFAULT-CLASS-MOUNT", true);
    }

    [DidReloadScripts]
    private static void LoadScriptAndMountWithName()
    {
        if (!EditorPrefs.GetBool("DEFAULT-CLASS-MOUNT")) return;
        EditorPrefs.SetBool("DEFAULT-CLASS-MOUNT", false);
        string className = EditorPrefs.GetString("DEFAULT-CLASS-NAME");
        EditorPrefs.DeleteKey("DEFAULT-CLASS-NAME");
        if (!m_target.gameObject.GetComponent(className))
            m_target.gameObject.AddComponent(GetTypeWithName(className));
    }

    /// <summary>
    /// Get a type with name.
    /// 根据名字获取一个类型
    /// </summary>
    public static Type GetTypeWithName(string typeName)
    {
        Assembly[] assmblies = AppDomain.CurrentDomain.GetAssemblies();

        for (int i = assmblies.Length - 1; i >= 0; i--)
        {
            if (assmblies[i].GetName().Name != "Assembly-CSharp") continue;

            Type[] __types = assmblies[i].GetTypes();

            for (int j = __types.Length - 1; j >= 0; j--)
            {
                if (__types[j].Name != typeName) continue;
                return __types[j];
            }
            break;
        }
        return null;
    }
}
 