using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GameFramework;
using UnityGameFramework.Editor.ResourceTools;

namespace Game.Main.Editor
{
    /// <summary>
    /// 资源组编辑数据
    /// </summary>
    public class ResourceGroupEditorData : ScriptableObject
    {
        public List<ResourceGroup> Group = new List<ResourceGroup>();

        public static ResourceGroupEditorData Load()
        {
            string path = "Assets/Deer/GameConfigs/ResourceGroupEditor.asset";

            ResourceGroupEditorData groupData= AssetDatabase.LoadAssetAtPath<ResourceGroupEditorData>(path);
            if (groupData == null)
            {
                groupData = ScriptableObject.CreateInstance<ResourceGroupEditorData>();
                AssetDatabase.CreateAsset(groupData, path);
            }
            return groupData;
        }
    }

    /// <summary>
    /// 资源组
    /// </summary>
    [Serializable]
    public class ResourceGroup
    {
        /// <summary>
        /// 启用分组
        /// </summary>
        public bool EnableGroup = true;
        /// <summary>
        /// 分组名
        /// </summary>
        public string GroupName = "Default";
        /// <summary>
        /// 分组备注
        /// </summary>
        public string GroupRemark;
        /// <summary>
        /// 打包规则
        /// </summary>
        public List<ResourceRule> Rules;

        public ResourceGroup()
        {
            EnableGroup = true;
            GroupName = "Default";
            Rules = new List<ResourceRule>();
        }

        public void SetGroupName(string groupName)
        {
            GroupName = groupName;
            if (Rules == null) return;
            for (int i = 0; i < Rules.Count; i++)
            {
                Rules[i].GroupName = groupName;
            }
        }
    }

    /// <summary>
    /// 资源规则
    /// </summary>
    [Serializable]
    public class ResourceRule
    {
        /// <summary>
        /// 使用这条规则
        /// </summary>
        public bool Enable;
        /// <summary>
        /// 所属分组
        /// </summary>
        public string GroupName;
        /// <summary>
        /// 规则名
        /// </summary>
        public string Name;
        /// <summary>
        /// 加载类型
        /// </summary>
        public LoadType LoadType;
        /// <summary>
        /// 是否包装
        /// </summary>
        public bool Packed;
        /// <summary>
        /// 文件系统
        /// </summary>
        public string FileSystem;
        /// <summary>
        /// 变体
        /// </summary>
        public string Variant;
        /// <summary>
        /// 资源文件夹目录
        /// </summary>
        public string AssetsDirectoryPath;
        /// <summary>
        /// 资源筛选类型
        /// </summary>
        public ResourceFilterType FilterType;

        public ResourceRule()
        {
            Enable = true;
            GroupName = string.Empty;
            Name = string.Empty;
            Packed = true;
            FileSystem = string.Empty;
            Variant = null;
            LoadType = LoadType.LoadFromFile;
            AssetsDirectoryPath = string.Empty;
            FilterType = ResourceFilterType.Root;
        }
    }

    /// <summary>
    /// 资源筛选类型
    /// </summary>
    public enum ResourceFilterType
    {
        Root,
        Children,
        ChildrenFoldersOnly,
        ChildrenFilesOnly,
    }
}