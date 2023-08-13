using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityGameFramework.Editor.ResourceTools;
using GFResource = UnityGameFramework.Editor.ResourceTools.Resource;
using GameFramework;

namespace Game.Main.Editor
{
    /// <summary>
    /// 资源分组编辑程序
    /// </summary>
    internal static class ResourceGroupEditorUtility
    {
        // 资源规则
        private static ResourceCollection m_ResourceCollection;
        // 资源筛选控制
        private static ResourceEditorController m_ResourceEditorController;
        // 排除的类型
        private static string[] m_SourceAssetExceptTypeFilterGUIDArray;
        // 排除的标签
        private static string[] m_SourceAssetExceptLabelFilterGUIDArray;
        // 文件搜索模式
        private const string m_SearchPattern = "*.*";
        // 根路径
        private static string m_RootPath;
        // 资源组数据
        private static ResourceGroupEditorData m_GroupData = null;

        /// <summary>
        /// 刷新资源收集
        /// </summary>
        /// <param name="groupData"></param>
        public static void RefreshResourceCollection(ResourceGroupEditorData groupData=null)
        {
            m_GroupData = groupData;

            if (m_ResourceEditorController == null)
            {
                m_ResourceEditorController = new ResourceEditorController();
                m_ResourceEditorController.Load();
            }
            if (m_GroupData == null)
            {
                m_GroupData = ResourceGroupEditorData.Load();
            }

            m_RootPath= Utility.Text.Format("{0}/{1}", Application.dataPath, m_ResourceEditorController.SourceAssetRootPath.Replace("Assets/", string.Empty));

            m_SourceAssetExceptTypeFilterGUIDArray = AssetDatabase.FindAssets(m_ResourceEditorController.SourceAssetExceptTypeFilter);
            m_SourceAssetExceptLabelFilterGUIDArray = AssetDatabase.FindAssets(m_ResourceEditorController.SourceAssetExceptLabelFilter);

            AnalysisResourceFilters();
            if (SaveCollection())
            {
                Debug.Log("Refresh ResourceCollection.xml success");
            }
            else
            {
                Debug.Log("Refresh ResourceCollection.xml fail");
            }
        }

        private static bool HasResource(string name, string Variant)
        {
            return m_ResourceCollection.HasResource(name, Variant);
        }

        private static bool AddResource(string name, string Variant, string FileSystem,
            LoadType LoadType, bool Packed, string[] resourceGroups)
        {
            return m_ResourceCollection.AddResource(name, Variant, FileSystem, LoadType,
                Packed, resourceGroups);
        }

        private static bool RenameResource(string oldName, string oldVariant,
            string newName, string newVariant)
        {
            return m_ResourceCollection.RenameResource(oldName, oldVariant,
                newName, newVariant);
        }

        private static bool AssignAsset(string assetGuid, string resourceName, string resourceVariant)
        {
            if (m_ResourceCollection.AssignAsset(assetGuid, resourceName, resourceVariant))
            {
                return true;
            }

            return false;
        }

        private static void AnalysisResourceFilters()
        {
            m_ResourceCollection = new ResourceCollection();
            List<string> signedAssetBundleList = new List<string>();

            for (int x = 0; x < m_GroupData.Group.Count; x++)
            {
                ResourceGroup resourceGroup = m_GroupData.Group[x];
                if (resourceGroup.EnableGroup)
                {
                    for (int y = 0; y < resourceGroup.Rules.Count; y++)
                    {
                        ResourceRule resourceRule = resourceGroup.Rules[y];
                        if (!resourceRule.Enable) continue;
                        if (string.IsNullOrEmpty(resourceRule.Variant)) resourceRule.Variant = null;

                        switch (resourceRule.FilterType)
                        {
                            case ResourceFilterType.Root:
                                {
                                    string relativeDirectoryName = resourceRule.AssetsDirectoryPath.Replace(m_ResourceEditorController.SourceAssetRootPath + "/", "");
                                    ApplyResourceFilter(ref signedAssetBundleList, resourceRule, Utility.Path.GetRegularPath(relativeDirectoryName));
                                }
                                break;

                            case ResourceFilterType.Children:
                                {
                                    FileInfo[] assetFiles =
                                            new DirectoryInfo(resourceRule.AssetsDirectoryPath).GetFiles(m_SearchPattern, SearchOption.AllDirectories);
                                    foreach (FileInfo file in assetFiles)
                                    {
                                        if (file.Extension.Contains("meta"))
                                            continue;

                                        string relativeAssetName = file.FullName.Substring(m_RootPath.Length + 1);
                                        string relativeAssetNameWithoutExtension =
                                            Utility.Path.GetRegularPath(
                                                relativeAssetName.Substring(0, relativeAssetName.LastIndexOf('.')));

                                        string assetName = Path.Combine(m_ResourceEditorController.SourceAssetRootPath, relativeAssetName);
                                        string assetGUID = AssetDatabase.AssetPathToGUID(assetName);

                                        if (!m_SourceAssetExceptTypeFilterGUIDArray.Contains(assetGUID) &&
                                            !m_SourceAssetExceptLabelFilterGUIDArray.Contains(assetGUID))
                                        {
                                            ApplyResourceFilter(ref signedAssetBundleList, resourceRule,
                                                relativeAssetNameWithoutExtension, assetGUID);
                                        }
                                    }
                                }
                                break;

                            case ResourceFilterType.ChildrenFoldersOnly:
                                {
                                    DirectoryInfo[] assetDirectories =
                                        new DirectoryInfo(resourceRule.AssetsDirectoryPath).GetDirectories();
                                    foreach (DirectoryInfo directory in assetDirectories)
                                    {
                                        string relativeDirectoryName =
                                            directory.FullName.Substring(m_RootPath.Length + 1);

                                        ApplyResourceFilter(ref signedAssetBundleList, resourceRule,
                                            Utility.Path.GetRegularPath(relativeDirectoryName), string.Empty,
                                            directory.FullName);
                                    }
                                }
                                break;

                            case ResourceFilterType.ChildrenFilesOnly:
                                {
                                    DirectoryInfo[] assetDirectories = new DirectoryInfo(resourceRule.AssetsDirectoryPath).GetDirectories();
                                    foreach (DirectoryInfo directory in assetDirectories)
                                    {
                                        FileInfo[] assetFiles =
                                                new DirectoryInfo(directory.FullName).GetFiles(m_SearchPattern, SearchOption.AllDirectories);
                                        foreach (FileInfo file in assetFiles)
                                        {
                                            if (file.Extension.Contains("meta"))
                                                continue;
                                            string relativeAssetName =
                                                file.FullName.Substring(m_RootPath.Length + 1);
                                            string relativeAssetNameWithoutExtension =
                                                Utility.Path.GetRegularPath(
                                                    relativeAssetName.Substring(0, relativeAssetName.LastIndexOf('.')));

                                            string assetName = Path.Combine(m_ResourceEditorController.SourceAssetRootPath, relativeAssetName);
                                            string assetGUID = AssetDatabase.AssetPathToGUID(assetName);

                                            if (!m_SourceAssetExceptTypeFilterGUIDArray.Contains(assetGUID) &&
                                                !m_SourceAssetExceptLabelFilterGUIDArray.Contains(assetGUID))
                                            {
                                                ApplyResourceFilter(ref signedAssetBundleList, resourceRule,
                                                    relativeAssetNameWithoutExtension, assetGUID);
                                            }
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }
            }
        }

        private static void ApplyResourceFilter(ref List<string> signedResourceList, ResourceRule resourceRule,
            string resourceName, string singleAssetGUID = "", string childDirectoryPath = "")
        {
            if (!signedResourceList.Contains(Path.Combine(resourceRule.AssetsDirectoryPath, resourceName)))
            {
                signedResourceList.Add(Path.Combine(resourceRule.AssetsDirectoryPath, resourceName));

                GFResource[] resources = m_ResourceCollection.GetResources();
                foreach (GFResource oldResource in resources)
                {
                    if (oldResource.Name == resourceName && string.IsNullOrEmpty(oldResource.Variant))
                    {
                        RenameResource(oldResource.Name, oldResource.Variant,
                            resourceName, resourceRule.Variant);
                        break;
                    }
                }

                if (!HasResource(resourceName, resourceRule.Variant))
                {
                    if (string.IsNullOrEmpty(resourceRule.FileSystem))
                    {
                        resourceRule.FileSystem = null;
                    }

                    AddResource(resourceName, resourceRule.Variant, resourceRule.FileSystem,
                        resourceRule.LoadType, resourceRule.Packed,
                        new string[] { resourceRule.GroupName });
                }

                switch (resourceRule.FilterType)
                {
                    case ResourceFilterType.Root:
                    case ResourceFilterType.ChildrenFoldersOnly:
                        if (childDirectoryPath == "")
                        {
                            childDirectoryPath = resourceRule.AssetsDirectoryPath;
                        }
                        FileInfo[] assetFiles =
                                new DirectoryInfo(childDirectoryPath).GetFiles(m_SearchPattern, SearchOption.AllDirectories);
                        foreach (FileInfo file in assetFiles)
                        {
                            if (file.Extension.Contains("meta"))
                                continue;

                            string assetName = Path.Combine(m_ResourceEditorController.SourceAssetRootPath, file.FullName.Substring(m_RootPath.Length + 1));

                            string assetGUID = AssetDatabase.AssetPathToGUID(assetName);

                            if (!m_SourceAssetExceptTypeFilterGUIDArray.Contains(assetGUID) &&
                                !m_SourceAssetExceptLabelFilterGUIDArray.Contains(assetGUID))
                            {
                                AssignAsset(assetGUID, resourceName,
                                    resourceRule.Variant);
                            }
                        }

                        break;

                    case ResourceFilterType.Children:
                    case ResourceFilterType.ChildrenFilesOnly:
                        {
                            AssignAsset(singleAssetGUID, resourceName,
                                resourceRule.Variant);
                        }
                        break;
                }
            }
        }

        private static bool SaveCollection()
        {
            return m_ResourceCollection.Save();
        }
    }
}