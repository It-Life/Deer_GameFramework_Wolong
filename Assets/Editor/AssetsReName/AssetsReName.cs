using UnityEngine;
using UnityEditor;
using System.IO;
using NPinyin;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Deer.Editor
{
    /// <summary>
    /// 资源重命名工具
    /// </summary>
    public class AssetsReName : EditorWindow
    {
        private static int index = 0;//序号  
        private static int total = 0;//最大进度

        private readonly List<ChildViewBase> listViews = new List<ChildViewBase>();

        private int optionIndex = 0;

        private Vector2 m_LeftScroll;
        private Vector2 m_RightScroll;
        protected string leftViewTitle = "内容";

        protected class Styles
        {
            ////////public GUIContent m_WarningContent = new GUIContent(string.Empty, EditorGUIUtility.LoadRequired("Builtin Skins/Icons/console.warnicon.sml.png") as Texture2D);
            public GUIStyle m_PreviewBox = new GUIStyle("OL Box");
            public GUIStyle m_PreviewTitle = new GUIStyle("OL Title");
            public GUIStyle m_LoweredBox = new GUIStyle("TextField");
            public GUIStyle m_HelpBox = new GUIStyle("helpbox");
            public Styles()
            {
                m_LoweredBox.padding = new RectOffset(1, 1, 1, 1);
            }
        }
        protected static Styles m_Styles;
        private string[] optionsTitle
        {
            get
            {
                if (listViews.Count == 0)
                {
                    return new string[1] { "暂无选项" };
                }
                string[] result = listViews.Select(item => item.optionTitle).ToArray();
                return result;
            }
        }

        public AssetsReName()
        {
            //listViews.Add(new AssetsRenameToPinYin() { optionTitle = "选项" });
            var views = GetClassChilds<ChildViewBase>();
            listViews.AddRange(views);

            var testIFC = GetIFCChilds<IChildViewRightGUIContent>();
            Debug.Log("能获取到继承接口的数量吗？" + testIFC.Count);

            //////Debug.Log("初始化了几次？？");
            Debug.Log("是否等于：" + (typeof(RenameToPinYin) == typeof(IChildViewLeftGUIContent)));
        }

        List<T> GetClassChilds<T>() where T : class
        {
            //var types = Assembly.GetEntryAssembly().GetTypes();
            var types = Assembly.GetCallingAssembly().GetTypes();
            types = types.Where(item => item.BaseType == typeof(T)).ToArray();

            var aType = typeof(T);
            Debug.Log(aType.FullName);
            List<T> alist = new List<T>();
            foreach (var type in types)
            {
                var baseType = type.BaseType;  //获取基类
                while (baseType != null)  //获取所有基类
                {
                    //Debug.Log(baseType.Name);
                    if (baseType.Name == aType.Name)
                    {
                        System.Type objtype = System.Type.GetType(type.FullName, true);
                        object obj = System.Activator.CreateInstance(objtype);
                        if (obj != null)
                        {
                            T info = obj as T;
                            alist.Add(info);
                        }
                        break;
                    }
                    else
                    {
                        baseType = baseType.BaseType;
                    }
                }
            }
            return alist;
        }

        //////List<T> GetIFCChilds<T>() where T : class
        //////{
        //////    //var types = Assembly.GetEntryAssembly().GetTypes();
        //////    var types = Assembly.GetCallingAssembly().GetTypes();
        //////    types = types.Where(item => item.GetInterface(typeof(T).ToString()) != null).ToArray();//types = types.Where(item => item.BaseType == typeof(T)).ToArray();
        //////    //Debug.Log( T + "长度：：：" + types.Length);
        //////    return new List<T>();
        //////    var aType = typeof(T);
        //////    Debug.Log(aType.FullName);
        //////    List<T> alist = new List<T>();
        //////    foreach (var type in types)
        //////    {
        //////        var baseType = type.GetInterface(name);//type.BaseType;  //获取基类
        //////        while (baseType != null)  //获取所有基类
        //////        {
        //////            //Debug.Log(baseType.Name);
        //////            if (baseType.Name == aType.Name)
        //////            {
        //////                System.Type objtype = System.Type.GetType(type.FullName, true);
        //////                object obj = System.Activator.CreateInstance(objtype);
        //////                if (obj != null)
        //////                {
        //////                    T info = obj as T;
        //////                    alist.Add(info);
        //////                }
        //////                break;
        //////            }
        //////            else
        //////            {
        //////                baseType = baseType.BaseType;
        //////            }
        //////        }
        //////    }
        //////    return alist;
        //////}
        List<T> GetIFCChilds<T>() where T : class
        {//参考链接： https://www.cnblogs.com/ZhiXing-Blogs/p/7380649.html
            var types = Assembly.GetCallingAssembly().GetTypes();
            var aType = typeof(T);
            Debug.Log(aType.FullName);
            List<T> result = new List<T>();
            var typess = Assembly.GetCallingAssembly().GetTypes();  //获取所有类型
            foreach (var t in typess)
            {
                System.Type[] tfs = t.GetInterfaces();  //获取该类型的接口
                foreach (var tf in tfs)
                {
                    if (tf.FullName == aType.FullName)  //判断全名，是否在一个命名空间下面
                    {
                        T a = System.Activator.CreateInstance(t) as T;
                        result.Add(a);
                    }
                }
            }
            //////Debug.Log(result.Count);
            //////foreach (var item in ass)
            //////{
            //////    item.a();  //调用所有继承该接口的类中的方法
            //////}
            return result;
        }

        //////List<T> GetClassInterface<T>()
        //////{
        //////    System.Type classType = typeof(System.Int32);
        //////    System.Type[] interfaces = classType.GetInterfaces();
        //////    foreach (System.Type eachType in interfaces)
        //////    {
        //////        Debug.Log(eachType.ToString());
        //////    }
        //////}


        #region /*——EditorWindow Base Behaviour——*/

        void OnSelectionChange()
        {
            this.Repaint();
        }

        private void OnDisable()
        {
            Debug.Log("===》OnDisable");
        }

        #endregion

        #region /*——GUI——*/

        [MenuItem("DeerTools/Asset/Asset ReName _&#_1")]
        public static void NewWindow_AssetsReName()
        {
            GetWindow<AssetsReName>(true, "资源重命名");
        }
        private void OnGUI()
        {
            if (m_Styles == null)
                m_Styles = new Styles();

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Space(10);

                LeftGUI();
                GUILayout.Space(10);

                EditorGUILayout.BeginVertical();
                {
                    RightGUI();

                    GUILayout.Space(10);
                    //GUILayout.FlexibleSpace ();
                    //////CreateAndCancelButtonsGUI();
                }
                EditorGUILayout.EndVertical();

                GUILayout.Space(10);
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);
        }
        private void LeftGUI()
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(Mathf.Max(position.width * 0.4f, position.width - 380f)));
            {
                Rect previewHeaderRect = GUILayoutUtility.GetRect(new GUIContent("标题ABC"), m_Styles.m_PreviewTitle);
                m_LeftScroll = EditorGUILayout.BeginScrollView(m_LeftScroll, m_Styles.m_PreviewBox);
                {
                    LeftGUIContent();
                }
                EditorGUILayout.EndScrollView();

                GUI.Label(previewHeaderRect, new GUIContent(leftViewTitle), m_Styles.m_PreviewTitle);
                GUILayout.Space(4);
            }
            EditorGUILayout.EndVertical();
        }

        protected void LeftGUIContent()
        {
            Object[] m_objects = Selection.GetFiltered(typeof(Object), SelectionMode.Deep);
            //////Object[] m_objects1 = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);
            //////Object[] m_objects2 = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
            //////Object[] m_objects3 = Selection.GetFiltered(typeof(Object), SelectionMode.Editable);
            //////Object[] m_objects4 = Selection.GetFiltered(typeof(Object), SelectionMode.ExcludePrefab);
            //////Object[] m_objects5 = Selection.GetFiltered(typeof(Object), SelectionMode.OnlyUserModifiable);
            //////Object[] m_objects6 = Selection.GetFiltered(typeof(Object), SelectionMode.TopLevel);
            //////Object[] m_objects7 = Selection.GetFiltered(typeof(Object), SelectionMode.Unfiltered);

            GUILayout.Label("已选中资源：" + m_objects.Length + " 个");
            GUILayout.Space(16);
            for (int i = 0; i < m_objects.Length; i++)
            {
                Object gmObj = m_objects[i];
                EditorGUILayout.ObjectField(gmObj, typeof(GameObject), true);
            }
        }
        private void RightGUI()
        {
            m_RightScroll = EditorGUILayout.BeginScrollView(m_RightScroll);
            {
                RightGUIContent();
            }
            EditorGUILayout.EndScrollView();
        }
        protected void RightGUIContent()
        {
            GUI.skin.button.fontSize = 15;
            GUI.skin.label.fontSize = 15;

            optionIndex = EditorGUILayout.Popup("测试下拉菜单", optionIndex, optionsTitle);
            if (optionIndex >= 0 && optionIndex < listViews.Count)
            {
                IChildViewRightGUIContent iRightGUI = listViews[optionIndex] as IChildViewRightGUIContent;
                if (iRightGUI != null)
                {
                    iRightGUI.ChildRightGUIContent();
                }
            }
            return;
        }

        #endregion



        /// <summary>资源重命名—中文名转拼音
        /// </summary>
        public class RenameToPinYin : ChildViewBase, IChildViewRightGUIContent
        {
            public RenameToPinYin()
            {
                this.optionTitle = "资源重命名—中文名转拼音";
            }

            public bool ChildRightGUIContent()
            {
                if (GUILayout.Button("重命名为拼音"))
                {
                    Debug.Log("哈哈哈？");
                    RenameAll();
                }
                return false;
            }

            #region/*——Logic——*/
            public static void RenameAll()
            {
                Object[] m_objects = Selection.GetFiltered(typeof(Object), SelectionMode.Deep);
                Queue<Object> queObj = new Queue<Object>(m_objects);
                if (queObj.Count == 0)
                {
                    Debug.LogError("请选中需要重命名的对象");
                    return;
                }
                index = 0;
                total = m_objects.Length;
                Debug.Log("选择了" + m_objects.Length);
                EditorApplication.update = delegate ()
                {
                    if (queObj.Count > 0)
                    {
                        Object obj = queObj.Dequeue();
                        ObjectReName(obj);
                        index++;
                    }
                    EditorUtility.DisplayProgressBar("资源中文名字重命名为拼音中...", "\t\t进度：" + index + "/" + total, (float)index / total);
                    if (index >= total || queObj.Count == 0)
                    {//index >= total ||
                        Debug.Log("转换结束!");
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                        EditorUtility.ClearProgressBar();
                        EditorApplication.update = null;
                    }
                    //Debug.Log("index2===》" + index);
                };

            }

            private static void ObjectReName(Object item)
            {
                if (item.GetType() == typeof(GameObject))
                {
                    GameObject obj = item as GameObject;
                    //Debug.Log("===》" + obj.name);
                    string newName = GetPinYin(item.name);
                    obj.name = newName;
                    return;
                }
                if (Path.GetExtension(AssetDatabase.GetAssetPath(item)) != "")
                {
                    string path = AssetDatabase.GetAssetPath(item);
                    string newName = GetPinYin(item.name);
                    AssetDatabase.RenameAsset(path, newName);
                }
            }

            private static string GetPinYin(string value)
            {
                StringBuilder result = new StringBuilder();
                for (int i = 0; i < value.Length; i++)
                {
                    bool isGBK = value[i].IsGBK();
                    if (i != 0 && isGBK)
                    {
                        result.Append(Pinyin.GetPinyin(value[i]).ToINITCAP());
                    }
                    else
                    {
                        result.Append(Pinyin.GetPinyin(value[i]));
                    }
                }

                if (result.ToString().IndexOf("anNiu") >= 0)
                {
                    result = result.Replace("anNiu", "");
                    result.Insert(0, "btn_");
                }
                else if (result.ToString().IndexOf("AnNiu") >= 0)
                {
                    result = result.Replace("AnNiu", "");
                    result.Insert(0, "btn_");
                }
                return result.ToString();
            }
            #endregion
        }

        /// <summary>资源重命名—名字增加数字
        /// </summary>
        public class RenameAddNumber : ChildViewBase, IChildViewRightGUIContent
        {
            internal bool isEndAdd = true;
            internal string strStartNum = "0";
            public RenameAddNumber()
            {
                this.optionTitle = "资源重命名—名字增加数字";
            }

            public bool ChildRightGUIContent()
            {
                //throw new NotImplementedException();
                strStartNum = GUILayout.TextField(strStartNum);
                int startNum = 0;
                int.TryParse(strStartNum, out startNum);
                if (startNum == 0)
                {
                    strStartNum = startNum.ToString();
                }

                GUILayout.BeginHorizontal();
                {
                    isEndAdd = GUILayout.Toggle(isEndAdd, isEndAdd ? "从末尾添加数字" : "从开始添加数字");
                }
                GUILayout.EndHorizontal();
                if (GUILayout.Button("名字加数字结尾"))
                {
                    Call(this.isEndAdd, startNum);
                }
                return false;
            }

            public static void Call(bool isEndAdd = true, int startNumber = 0)
            {
                Object[] objects = Selection.GetFiltered(typeof(Object), SelectionMode.Deep);
                GameObject[] rankObjs = objects.Where(item => item.GetType() == typeof(GameObject))
                                               .Select(item => item as GameObject)
                                               .OrderBy(item => item.transform.GetSiblingIndex()).ToArray();
                for (int i = 0; i < rankObjs.Length; i++)
                {
                    if (rankObjs[i].GetType() == typeof(UnityEngine.GameObject))
                    {
                        GameObject gmObj = rankObjs[i];// rankObjs[i] as GameObject;
                        if (isEndAdd)
                        {
                            gmObj.name = gmObj.name + startNumber;
                        }
                        else
                        {
                            gmObj.name = startNumber + gmObj.name;
                        }
                        startNumber++;
                    }
                }
            }
        }

        /// <summary>资源重命名—替换名字的部分字符
        /// </summary>
        public class RenameReplace : ChildViewBase, IChildViewRightGUIContent
        {
            internal string oldString = "";
            internal string newString = "";
            internal bool isCNToNumber = false;
            internal bool isNumberToCN = false;
            public RenameReplace()
            {
                this.optionTitle = "资源重命名—替换名字的部分字符";
            }

            public bool ChildRightGUIContent()
            {
                //throw new NotImplementedException();

                oldString = GUILayout.TextField(oldString);
                GUILayout.Label("↑ 替换为 ↓");
                newString = GUILayout.TextField(newString);

                if (GUILayout.Button("替换字符"))
                {
                    Call(this.oldString, this.newString);
                }

                //isCNToNumber = GUILayout.Toggle(isCNToNumber, "中文数字转为阿拉伯数字");
                //isCNToNumber = GUILayout.Toggle(isCNToNumber, "中文数字转为阿拉伯数字");
                return false;
            }

            public static void Call(string oldString, string newString)
            {
                Object[] objects = Selection.GetFiltered(typeof(Object), SelectionMode.Deep);
                for (int i = 0; i < objects.Length; i++)
                {
                    Object item = objects[i];
                    if (objects[i].GetType() == typeof(UnityEngine.GameObject))
                    {
                        GameObject gmObj = objects[i] as GameObject;
                        gmObj.name = gmObj.name.Replace(oldString, newString);// + startNumber;
                    }
                    else if (Path.GetExtension(AssetDatabase.GetAssetPath(item)) != "")
                    {
                        string path = AssetDatabase.GetAssetPath(item);
                        string newName = item.name.Replace(oldString, newString);
                        AssetDatabase.RenameAsset(path, newName);
                    }
                }
            }

            //public static string GetNumberCN(string number)
            //{
            //    switch (number)
            //    {
            //        case "一": return "1";
            //        case "二": return "2";
            //        case "三": return "3";
            //        case "四": return "4";
            //        case "五": return "5";
            //        case "六": return "6";
            //        case "七": return "7";
            //        case "八": return "8";
            //        case "九": return "9";
            //        case "零": return "0";
            //        default:
            //            break;
            //    }
            //}
        }


        #region /*——Base Class  Or Interface  Model——*/

        public class ChildViewBase
        {
            public string optionTitle = "选项标题";
        }

        public interface IChildViewLeftGUIContent
        {
            bool ChildLeftGUIContent();
        }

        public interface IChildViewRightGUIContent
        {
            bool ChildRightGUIContent();
        }

        #endregion
    }

    /// <summary>string 帮助
    /// </summary>
    public static class StringConvert
    {
        /// <summary>首字母大写
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToINITCAP(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return string.Empty;

            char[] s = str.ToCharArray();
            char c = s[0];

            if ('a' <= c && c <= 'z')
                c = (char)(c & ~0x20);

            s[0] = c;

            return new string(s);
        }

        /// <summary>是否中文
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static bool IsGBK(this char ch)
        {
            byte[] byte_len = System.Text.Encoding.Default.GetBytes(ch.ToString());
            if (byte_len.Length >= 2) { return true; }
            return false;
        }
    }

}