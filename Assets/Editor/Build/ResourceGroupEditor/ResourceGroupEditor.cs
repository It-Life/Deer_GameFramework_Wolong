using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GameFramework;
using UnityGameFramework.Editor.ResourceTools;

namespace Game.Main.Editor
{
    public class ResourceGroupEditor : MenuTreeEditorWindow
    {
        private MenuTreeView<ResourceGroup> m_MenuTreeView;
        private MenuTreeViewItem<ResourceGroup> m_SelectedItem;
        private ResourceRuleTableView<ResourceRule> m_RuleTableView;
        private List<TableColumn<ResourceRule>> m_RuleColumns;
        private ResourceEditorController m_ResourceEditorController;

        private ResourceGroupEditorData m_GroupData;
        private string m_SelectRootPath;
        private readonly float m_AddGroupRectHeight = 70f;
        private Rect m_ToolbarRect = new Rect();
        private Rect m_AddGroupRect = new Rect();
        private Rect m_RuleRect = new Rect();
        private GUIStyle m_LineStyle;
        private GUIStyle m_FolderBtnStyle;
        private GUIContent m_FolderBtnContent;

        // 获取当前组
        public ResourceGroup CurrentGroup
        {
            get { return m_SelectedItem == null ? null : m_SelectedItem.Data; }
        }

        [MenuItem("Game Framework/Resource Tools/Resource Group Editor", false, 47)]
        public static void OpenWindow()
        {
            var window = GetWindow<ResourceGroupEditor>("资源分组");
            window.minSize = new Vector2(1000, 600);
        }


        protected override void OnEnable()
        {
            base.OnEnable();

            if (m_ResourceEditorController == null)
            {
                m_ResourceEditorController = new ResourceEditorController();
                m_ResourceEditorController.Load();
            }

            m_SelectRootPath = Utility.Text.Format("{0}/{1}", Application.dataPath, m_ResourceEditorController.SourceAssetRootPath.Replace("Assets/", string.Empty));
            // 菜单树视图
            m_MenuTreeView = new MenuTreeView<ResourceGroup>(false, true, true);
            {
                m_MenuTreeView.onDrawFoldout = DrawFoldoutCallback;
                m_MenuTreeView.onDrawRowContent = DrawMenuRowContentCallback;
                m_MenuTreeView.onSelectionChanged = SelectionChanged;
            }

            if (m_RuleColumns == null) m_RuleColumns = GetRuleColumns();
            // 规则表实体
            m_RuleTableView = new ResourceRuleTableView<ResourceRule>(null, m_RuleColumns);
            {
                m_RuleTableView.OnRightAddRow = RuleTreeViewRightAddRowCallback;
                //m_RuleTableView.OnValidDragDrop=
            }
            GetData();

            for (int i = 0; i < m_GroupData.Group.Count; i++)
            {
                ResourceGroup resourceGroup = m_GroupData.Group[i];
                m_MenuTreeView.AddItem(GetDisplayName(resourceGroup), resourceGroup);
            }
        }

        protected override void OnGUI()
        {
            GUIToolbar();
            base.OnGUI();
        }

        private void GUIToolbar()
        {
            m_ToolbarRect = new Rect(0, 0, position.width, 30);
            {
                GUILayout.BeginHorizontal("Toolbar", GUILayout.Height(30));
                {
                    if (GUILayout.Button("读取XML配置"))
                    {

                    }

                    if (GUILayout.Button("保存到XML配置"))
                    {
                        SaveResourceCollectionXML();
                    }
                }
                GUILayout.EndHorizontal();
            }
        }

        protected override void OnGUIMenuTree()
        {
            m_MenuRect = new Rect(0, m_ToolbarRect.height, m_MenuTreeWidth, position.height - m_AddGroupRectHeight - m_ToolbarRect.height);
            m_AddGroupRect = new Rect(0, m_ToolbarRect.height + m_MenuRect.height, m_MenuTreeWidth, m_AddGroupRectHeight);

            if (m_LineStyle == null) m_LineStyle = new GUIStyle("EyeDropperHorizontalLine");
            if (m_FolderBtnStyle == null) m_FolderBtnStyle = new GUIStyle("SettingsIconButton");
            if (m_FolderBtnContent == null) m_FolderBtnContent = new GUIContent(SourceFolder.Icon);
            m_MenuTreeView.OnGUI(m_MenuRect);

            Rect btnRect = new Rect(m_AddGroupRect.width * 0.15f, m_MenuRect.height + (m_AddGroupRectHeight - 10f), 50f, 20f);
            if (GUI.Button(btnRect, "+"))
            {
                ResourceGroup resourceGroup = new ResourceGroup();
                m_GroupData.Group.Add(resourceGroup);
                m_MenuTreeView.AddItem(GetDisplayName(resourceGroup), resourceGroup);
            }
            btnRect.Set(m_AddGroupRect.width * 0.85f - 50f, btnRect.y, btnRect.width, btnRect.height);
            if (GUI.Button(btnRect, "-"))
            {
                if (m_SelectedItem != null)
                {
                    m_MenuTreeView.RemoveItem(m_SelectedItem);
                    m_GroupData.Group.Remove(m_SelectedItem.Data);
                    m_SelectedItem = null;
                }
            }

            Color color = new Color(0.6f, 0.6f, 0.6f, 1.333f);
            if (EditorGUIUtility.isProSkin)
            {
                color.r = 0.12f;
                color.g = 0.12f;
                color.b = 0.12f;
            }

            if (Event.current.type != EventType.Repaint)
                return;

            Color orgColor = GUI.color;
            GUI.color = GUI.color * color;
            //GUI.DrawTexture(new Rect(m_AddGroupRect.x, m_AddGroupRect.y, m_AddGroupRect.width, 1), EditorGUIUtility.whiteTexture);
            //GUI.DrawTexture(new Rect(m_AddGroupRect.x, m_AddGroupRect.yMax - 1, m_AddGroupRect.width, 1), EditorGUIUtility.whiteTexture);
            GUI.DrawTexture(new Rect(m_AddGroupRect.x, m_AddGroupRect.y + 1, 1, m_AddGroupRect.height - 2 * 1), EditorGUIUtility.whiteTexture);
            GUI.DrawTexture(new Rect(m_AddGroupRect.xMax - 1, m_AddGroupRect.y + 1, 1, m_AddGroupRect.height - 2 * 1), EditorGUIUtility.whiteTexture);

            GUI.color = orgColor;
        }

        protected override void OnGUISpace()
        {
            m_SpaceRect = new Rect(m_MenuTreeWidth, m_ToolbarRect.height, m_SpaceWidth, position.height - m_ToolbarRect.height);

            EditorGUIUtility.AddCursorRect(m_SpaceRect, MouseCursor.ResizeHorizontal);

            if (Event.current.type == EventType.MouseDown && m_SpaceRect.Contains(Event.current.mousePosition))
                m_ResizingHorizontalSplitter = true;

            if (m_ResizingHorizontalSplitter)
            {
                m_MenuTreeWidth = Event.current.mousePosition.x;
                m_SpaceRect.x = m_MenuTreeWidth;
                Repaint();
            }

            if (Event.current.type == EventType.MouseUp)
                m_ResizingHorizontalSplitter = false;

        }

        protected override void OnGUIContent()
        {
            m_ContentRect = new Rect(m_MenuTreeWidth + m_SpaceWidth, m_ToolbarRect.height, position.width - m_MenuTreeWidth - m_SpaceWidth, position.height - m_ToolbarRect.height);

            m_RuleRect.Set(m_ContentRect.x, 120f, m_ContentRect.width, m_ContentRect.height - 90);

            if (m_SelectedItem != null)
            {
                ResourceGroup group = m_SelectedItem.Data;
                GUILayout.BeginArea(m_ContentRect);
                {
                    if (group.EnableGroup != EditorGUILayout.Toggle("启用分组", group.EnableGroup))
                    {
                        group.EnableGroup = !group.EnableGroup;
                    }
                    string groupName = EditorGUILayout.DelayedTextField("分组名", group.GroupName);
                    string groupRemark = EditorGUILayout.TextField("分组备注", group.GroupRemark);

                    if (group.GroupName != groupName)
                    {
                        group.SetGroupName(groupName);
                        m_SelectedItem.displayName = GetDisplayName(group);
                    }
                    if (group.GroupRemark != groupRemark)
                    {

                        group.GroupRemark = groupRemark;
                        m_SelectedItem.displayName = GetDisplayName(group);
                    }
                }
                GUILayout.EndArea();

                //如果鼠标拖拽结束时，并且鼠标所在位置在文本输入框内 
                if (Event.current.type == EventType.DragExited && m_RuleRect.Contains(Event.current.mousePosition))
                {
                    if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0)
                    {
                        for (int i = 0; i < DragAndDrop.paths.Length; i++)
                        {
                            string path = DragAndDrop.paths[i];
                            if (IsFolderPath(path))
                                AddRuluRow(path);
                        }
                    }
                }

                m_RuleTableView.OnGUI(m_RuleRect);
            }
        }

        /// <summary>
        /// 菜单项选择改变
        /// </summary>
        /// <param name="selectedIds"></param>
        protected override void SelectionChanged(IList<int> selectedIds)
        {
            m_SelectedItem = m_MenuTreeView.GetItemById(selectedIds[0]);

            m_RuleTableView.SetTableViewData(m_SelectedItem.Data.Rules, m_RuleColumns);
        }

        /// <summary>
        /// 绘制行内容回调
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="row"></param>
        /// <param name="item"></param>
        /// <param name="label"></param>
        /// <param name="selected"></param>
        /// <param name="focused"></param>
        /// <param name="useBoldFont"></param>
        /// <param name="isPinging"></param>
        private void DrawMenuRowContentCallback(Rect rect, int row, MenuTreeViewItem<ResourceGroup> item, string label, bool selected, bool focused, bool useBoldFont, bool isPinging)
        {
            GUI.Box(rect, "", m_LineStyle);
            float space = 5f + item.depth * 15f;
            Rect lableRect = new Rect(rect.x + space, rect.y, rect.width - space, rect.height);
            EditorGUI.BeginDisabledGroup(!item.Data.EnableGroup);
            GUI.Label(lableRect, item.displayName);
            EditorGUI.EndDisabledGroup();
        }

        private void RuleTreeViewRightAddRowCallback()
        {
            AddRuluRow(string.Empty);
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        private void GetData()
        {
            if (m_GroupData == null)
                m_GroupData = ResourceGroupEditorData.Load();
        }

        /// <summary>
        /// 获取显示名字
        /// </summary>
        /// <param name="resourceGroup"></param>
        /// <returns></returns>
        private string GetDisplayName(ResourceGroup resourceGroup)
        {
            string displayName = string.IsNullOrEmpty(resourceGroup.GroupRemark) ? resourceGroup.GroupName :
                string.Format("{0}({1})", resourceGroup.GroupName, resourceGroup.GroupRemark);
            return displayName;
        }

        /// <summary>
        /// 获取规则列
        /// </summary>
        /// <returns></returns>
        private List<TableColumn<ResourceRule>> GetRuleColumns()
        {
            var ruleColumns = new List<TableColumn<ResourceRule>>();

            TableColumn<ResourceRule> column1 = CreateColumn(new GUIContent("Enable", "使用这条规则"),
                (cellRect, data, rowIndex, isSelected, isFocused) =>
                {
                    data.Enable = EditorGUI.Toggle(cellRect, data.Enable);
                }, 50, 50, 60);
            ruleColumns.Add(column1);

            TableColumn<ResourceRule> column2 = CreateColumn(new GUIContent("Name", "规则名（可为空）"),
                (cellRect, data, rowIndex, isSelected, isFocused) =>
                {
                    data.Name = EditorGUI.TextField(cellRect, data.Name);
                }, 100, 50, 150);
            ruleColumns.Add(column2);

            TableColumn<ResourceRule> column3 = CreateColumn(new GUIContent("LoadType", "加载类型"),
                (cellRect, data, rowIndex, isSelected, isFocused) =>
                {
                    data.LoadType = (LoadType)EditorGUI.EnumPopup(cellRect, data.LoadType);
                }, 150, 50, 200);
            ruleColumns.Add(column3);

            TableColumn<ResourceRule> column4 = CreateColumn(new GUIContent("Packed", "是否包装（这些资源将会跟随软件一起发布，作为基础资源）"),
                (cellRect, data, rowIndex, isSelected, isFocused) =>
                {
                    data.Packed = EditorGUI.Toggle(cellRect, data.Packed);
                }, 50, 50, 60);
            ruleColumns.Add(column4);

            TableColumn<ResourceRule> column5 = CreateColumn(new GUIContent("FileSystem", "文件系统（可为空）"),
                (cellRect, data, rowIndex, isSelected, isFocused) =>
                {
                    data.FileSystem = EditorGUI.TextField(cellRect, data.FileSystem);
                }, 100, 50, 150);
            ruleColumns.Add(column5);

            TableColumn<ResourceRule> column6 = CreateColumn(new GUIContent("Variant", "资源变体（可为空）"),
                (cellRect, data, rowIndex, isSelected, isFocused) =>
                {
                    data.Variant = EditorGUI.TextField(cellRect, data.Variant);
                }, 100, 50, 150);
            ruleColumns.Add(column6);

            TableColumn<ResourceRule> column7 = CreateColumn(new GUIContent("AssetsDirectoryPath", "资源文件夹目录"),
                (cellRect, data, rowIndex, isSelected, isFocused) =>
                {
                    Rect textFildRect = new Rect(cellRect.x, cellRect.y, cellRect.width - 20, cellRect.height);
                    Rect selectBtnRect = new Rect(cellRect.x + textFildRect.width, cellRect.y, 20, 20);
                    data.AssetsDirectoryPath = EditorGUI.TextField(textFildRect, data.AssetsDirectoryPath);
                    if (GUI.Button(selectBtnRect, m_FolderBtnContent, m_FolderBtnStyle))
                    {
                        string fullPath = EditorUtility.OpenFolderPanel("选择文件夹", m_SelectRootPath, "");
                        
                        if(!string.IsNullOrEmpty(fullPath))
                        {
                            data.AssetsDirectoryPath = FilterPath(fullPath);
                        }
                    }
                }, 300, 200, 400);
            ruleColumns.Add(column7);

            TableColumn<ResourceRule> column8 = CreateColumn(new GUIContent("FilterType", "资源筛选类型"),
                (cellRect, data, rowIndex, isSelected, isFocused) =>
                {
                    data.FilterType = (ResourceFilterType)EditorGUI.EnumPopup(cellRect, data.FilterType);
                }, 100, 50, 150);
            ruleColumns.Add(column8);

            return ruleColumns;
        }

        /// <summary>
        /// 创建列
        /// </summary>
        /// <param name="content"></param>
        /// <param name="drawCellMethod"></param>
        /// <param name="width"></param>
        /// <param name="minWidth"></param>
        /// <param name="maxWidth"></param>
        /// <param name="canSort"></param>
        /// <param name="autoResize"></param>
        /// <returns></returns>
        private TableColumn<ResourceRule> CreateColumn(GUIContent content, DrawCellMethod<ResourceRule> drawCellMethod,
            float width, float minWidth, float maxWidth, bool canSort = false, bool autoResize = true)
        {
            TableColumn<ResourceRule> column = new TableColumn<ResourceRule>();
            {
                column.headerContent = content;
                column.width = width;
                column.minWidth = minWidth;
                column.maxWidth = maxWidth;
                column.canSort = canSort;
                column.autoResize = autoResize;
                column.DrawCell = drawCellMethod;
            }
            return column;
        }


        /// <summary>
        /// 添加规则行
        /// </summary>
        /// <param name="path"></param>
        private void AddRuluRow(string path)
        {
            ResourceRule rule = new ResourceRule();
            rule.GroupName = m_SelectedItem.Data.GroupName;
            rule.AssetsDirectoryPath = path;
            m_RuleTableView.AddData(rule);
        }

        /// <summary>
        /// 是否是文件夹路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private bool IsFolderPath(string path)
        {
            if (Directory.Exists(path))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 筛选路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private string FilterPath(string path)
        {
            if (path.Contains(m_SelectRootPath))
            {
                int indexOf = path.IndexOf(m_ResourceEditorController.SourceAssetRootPath);
                path = path[indexOf..];
            }
            else
            {
                path = string.Empty;
                Debug.LogWarningFormat("选择的文件夹必须在 '{0}' 下", m_ResourceEditorController.SourceAssetRootPath);
            }

            return path;
        }

        
        private void OnDestroy()
        {
            // 关闭时保存
            EditorUtility.SetDirty(m_GroupData);
            AssetDatabase.SaveAssets();
        }


        private void ReadResourceCollectionXML()
        {

        }

        private void SaveResourceCollectionXML()
        {
            ResourceGroupEditorUtility.RefreshResourceCollection(m_GroupData);
        }
    }
}