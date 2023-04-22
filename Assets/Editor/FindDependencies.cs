using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Baiting
{
    public class FindDependencies : EditorWindow
    {
        //记录所有图片的GUID 原始数据 排序后的数据 引用数据  依赖数据
        public static Dictionary<string, Object> _RawData,_SortData;

        private List<string> _DependentData;

        private static Object _currentObj;

        private static UnityEngine.Object _currentSelectObj;

        //排序方式
        private readonly string[] _sortingMode = { "名字", "数量" };

        //排序类型
        private readonly string[] _sortType = { "升序", "倒序" };

        private string _checkName;

        private Vector2 _currenSelectViewScroll = Vector2.zero;

        private Vector2 _currenViewScroll = Vector2.zero;

        //指定路径
        private static string _folderPath;

        //排序方式下标
        private int _sortingModeIndex;

        //排序类型下标
        private int _sortTypeIndex;

        private Vector2 _texturesViewScroll = Vector2.zero;

        //分页下标
        private int _pagingIndex = 1;
        //每页显示多少个
        private int _showMaxCount = 20;
        private int _showStartIndex = 0;
        private int _showEndIndex = 0;
        private void OnGUI()
        {
            //固定窗口大小
            maxSize = new Vector2(1000, 800);
            minSize = new Vector2(1000, 800);

            EditorGUILayout.BeginHorizontal("box", GUILayout.Height(200));
            {
                GUILayout.BeginVertical();
                {
                    GUILayout.Space(20);
                    
                    //选择文件夹
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label("指定文件夹（可为空）:", GUILayout.Width(200));
                        _folderPath = GUILayout.TextField(_folderPath, GUILayout.Width(200));
                        if (GUILayout.Button("选择文件夹", GUILayout.Width(100)))
                        {
                            var path = EditorUtility.OpenFolderPanel("选择文件夹", Application.dataPath, "");
                            _folderPath = path.Split(Application.dataPath.Remove(Application.dataPath.Length - 6))[1];
                            EditorPrefs.SetString("_folderPath",_folderPath);
                        }

                        if (GUILayout.Button("加载资源", GUILayout.Width(100)))
                            LoadFile();
                    }
                    GUILayout.EndHorizontal();

                    //按照名字搜索资源
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label("图片名字:", GUILayout.Width(200));
                        _checkName = GUILayout.TextField(_checkName, GUILayout.Width(200));

                        if (GUILayout.Button("搜索", GUILayout.Width(100)))
                        {
                            int index = 0;
                            _pagingIndex = 1;
                            foreach (var value in _SortData)
                            {
                                if (value.Value.obj.name == _checkName)
                                {
                                    _currentObj = value.Value;
                                    break;
                                }
                                index++;
                                if (index %_showMaxCount == 0)
                                {
                                    _pagingIndex++;
                                }
                            }
                        }
                    }
                    GUILayout.EndHorizontal();

                    //排序方式
                    GUILayout.BeginVertical();
                    {
                        _sortingModeIndex = EditorGUILayout.Popup("排序方式:", _sortingModeIndex, _sortingMode,
                            GUILayout.Width(400));
                        _sortTypeIndex = EditorGUILayout.Popup("排序类型", _sortTypeIndex, _sortType, GUILayout.Width(400));
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal("box", GUILayout.Height(550));
            {
                #region MyRegion

                EditorGUILayout.BeginVertical("box", GUILayout.Width(maxSize.x / 6),GUILayout.Height(550));
                {
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        GUILayout.Label("当前文件夹下所有的图片资源",GUILayout.Height(20));
                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.Space(20);
                    GUILayout.BeginHorizontal();
                    {
                        //显示列表
                        GUILayout.Label("名字",GUILayout.Height(20));
                        GUILayout.FlexibleSpace();
                        GUILayout.Label("被引用数量",GUILayout.Height(20));
                    }
                    GUILayout.EndHorizontal();


                    if (_RawData != null && _RawData.Count != 0)
                    {
                        //排序类型
                        switch (_sortingModeIndex)
                        {
                            case 0:
                                _SortData = new Dictionary<string, Object>(_sortTypeIndex == 0
                                    ? _RawData.OrderBy(x => x.Value.obj.name)
                                    : _RawData.OrderByDescending(x => x.Value.obj.name));
                                break;
                            case 1:
                                _SortData = new Dictionary<string, Object>(_sortTypeIndex == 0
                                    ? _RawData.OrderBy(x => x.Value.count)
                                    : _RawData.OrderByDescending(x => x.Value.count));
                                break;
                        }

                        int index = 0;
                        foreach (var texture in _SortData)
                        {
                            if (index >= _showStartIndex && index < _showEndIndex)
                            {
                                GUILayout.BeginHorizontal();
                                {
                                    //显示列表
                                    if (GUILayout.Button(texture.Value.obj.name,Alignment(),GUILayout.Width(maxSize.x / 6 - 80),GUILayout.Height(22)))
                                    {
                                        _currentObj = texture.Value;
                                        Selection.activeObject = _currentObj.obj;
                                    }
                                    GUILayout.FlexibleSpace();
                                    GUILayout.Label(texture.Value.count.ToString(), GUILayout.Height(22));
                                }
                                GUILayout.EndHorizontal();
                            }
                            index++;
                        }
                    }
                }
                EditorGUILayout.EndVertical();
                #endregion

                #region MyRegion
                
                EditorGUILayout.BeginVertical("box", GUILayout.Width(maxSize.x / 6 ),GUILayout.Height(550));
                {
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        GUILayout.Label("图片都被那些资源引用",GUILayout.Height(20));
                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.Space(20);
                    GUILayout.BeginHorizontal();
                    {
                        //显示列表
                        GUILayout.Label("名字",GUILayout.Height(20));
                    }
                    GUILayout.EndHorizontal();
                    
                    _currenViewScroll = GUILayout.BeginScrollView(_currenViewScroll);
                    {
                        if (_currentObj != null)
                        {
                            if (_currentObj.targets.Count != 0)
                            {
                                foreach (var texture in _currentObj.targets)
                                {
                                    if (GUILayout.Button(texture.obj.name,Alignment(),GUILayout.Height(21)))
                                    {
                                        Selection.activeObject =  texture.obj;
                                        _currentSelectObj = texture.obj;
                                        
                                        _DependentData = new List<string>(
                                            AssetDatabase.GetDependencies(
                                                AssetDatabase.GetAssetPath(_currentSelectObj)));
                                    }
                                }
                            }
                            else
                            {
                                _currentSelectObj = null;
                                if (_DependentData != null)
                                    _DependentData.Clear();
                            }
                        }
                    }
                    GUILayout.EndScrollView();
                }
                EditorGUILayout.EndVertical();
                #endregion

                #region MyRegion
                EditorGUILayout.BeginVertical("box", GUILayout.Width(maxSize.x / 6 * 4),GUILayout.Height(550));
                {
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        GUILayout.Label("被引用的物体下所有的资源依赖",GUILayout.Height(20));
                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();
                    
                    
                    GUILayout.Space(20);
                    GUILayout.BeginHorizontal();
                    {
                        //显示列表
                        GUILayout.Label("名字",GUILayout.Height(20));
                    }
                    GUILayout.EndHorizontal();
                    
                    _currenSelectViewScroll = GUILayout.BeginScrollView(_currenSelectViewScroll);
                    {
                        if ( _DependentData != null && _DependentData.Count !=0)
                        {
                            foreach (var texture in _DependentData)
                            {
                                if (GUILayout.Button(texture,Alignment(),GUILayout.Height(21)))
                                {
                                    Selection.activeObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(texture);
                                }
                            }
                        }
                    }
                    GUILayout.EndScrollView();
                    
                }
                EditorGUILayout.EndVertical();
                #endregion
            }
            EditorGUILayout.EndHorizontal();
            

            EditorGUILayout.BeginHorizontal(GUILayout.Height(50));
            {
                EditorGUILayout.BeginHorizontal(GUILayout.Width(maxSize.x / 6));
                {
                    // 创建一个可伸缩空间，使其左右两边的控件水平居中
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("<", GUILayout.Width(20)))
                    {
                        _pagingIndex--;
                        _pagingIndex = _pagingIndex < 1 ? 1 : _pagingIndex;
                        
                    }
                    if (GUILayout.Button(">", GUILayout.Width(20)))
                    {
                        _pagingIndex++;
                        int maxShwoCount = (int)Math.Ceiling(_SortData.Count / (float)_showMaxCount);
                        _pagingIndex = _pagingIndex > maxShwoCount ? maxShwoCount : _pagingIndex;
                    }
                    
                    _showStartIndex = _pagingIndex == 1 ? 0 : (_pagingIndex - 1) * _showMaxCount;
                    _showEndIndex = _pagingIndex == 1 ? _showMaxCount : _pagingIndex * _showMaxCount;
                    
                    GUILayout.FlexibleSpace();
                    
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndHorizontal();
        }


        /// <summary>
        ///     打开窗口
        /// </summary>
        [MenuItem("DeerTools/Asset/Find Texture Reference", false, 0)]
        public static void OpenWindows()
        {
            var windows = GetWindow<FindDependencies>("查找依赖项");
            
            _folderPath = EditorPrefs.GetString("_folderPath");
            
        }

        private void LoadFile()
        {
            //初始化
            _pagingIndex = 1;
            _showStartIndex = 0;
            _showEndIndex = _showMaxCount;  
            if (_RawData == null)
                _RawData = new Dictionary<string, Object>();
            _RawData.Clear();
            if (_SortData == null)
                _SortData = new Dictionary<string, Object>();
            _SortData.Clear();
            
            //查找所有的图片
            var guids = AssetDatabase.FindAssets("t:Texture", new[] { _folderPath });

            foreach (var guid in guids)
            {
                //获取路径
                var path = AssetDatabase.GUIDToAssetPath(guid);
                //获取对象
                var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
                //添加到字典
                _RawData.Add(path, new Object(obj, path, guid, 0));
            }

            //查找所有对象
            var objs = AssetDatabase.FindAssets("t:GameObject", null);

            foreach (var guid in objs)
            {
                //获取路径
                var path = AssetDatabase.GUIDToAssetPath(guid);
                //获取对象
                var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
                //获取依赖
                var depen = AssetDatabase.GetDependencies(path);
                foreach (var item in depen)
                    //如果是图片
                    if (AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(item).GetType() == typeof(Texture2D))
                    {
                        if (_RawData.ContainsKey(item))
                        {
                            _RawData[item].count++;
                            _RawData[item].targets.Add(new Object(obj, path, guid, 0));
                        }
                    }
            }
        }

        private GUIStyle Alignment()
        {
            // 创建一个GUIStyle对象并设置其对齐方式为左对齐
            GUIStyle leftAlignedStyle = new GUIStyle(GUI.skin.button);
            leftAlignedStyle.alignment = TextAnchor.MiddleLeft;
            return leftAlignedStyle;
        }

        /// <summary>
        ///     获取依赖项
        /// </summary>
        /// <param name="obj"></param>
        private static UnityEngine.Object[] GetDependencies(UnityEngine.Object obj)
        {
            var dependencies = EditorUtility.CollectDependencies(new[] { obj });

            return dependencies;
        }
    }

    public class Object
    {
        public int count;
        public string guid;

        public UnityEngine.Object obj; //自身
        public string path;
        public List<Object> targets = new();

        public Object(UnityEngine.Object obj, string path, string guid, int count)
        {
            this.obj = obj;
            this.guid = guid;
            this.path = path;
            this.count = count;
        }
    }

    public class DeleteAssetMonitor : AssetModificationProcessor
    {
        public static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options)
        {
            Debug.Log("Deleting asset: " + assetPath);
            if (FindDependencies._RawData != null)
            {
                if (FindDependencies._RawData.ContainsKey(assetPath))
                {
                    FindDependencies._RawData.Remove(assetPath);
                }
            }
            return AssetDeleteResult.DidNotDelete;
        }
    }

}