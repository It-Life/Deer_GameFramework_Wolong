using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace AssetDanshari
{
    public class AssetReferenceTreeModel : AssetTreeModel
    {
        public override void SetDataPaths(string refPathStr, string pathStr, string commonPathStr)
        {
            base.SetDataPaths(refPathStr, pathStr, commonPathStr);
            assetPaths = refPathStr;

            var resFileList = GetResFileList();
            var guidList = GetGuidFromFileList(resFileList);
            var fileList = GetRefFileList();
            var searchRetList = GetSearchResultList(fileList.Count, guidList.Count);

            ThreadDoFilesTextSearchReplace(fileList, guidList, String.Empty, searchRetList);
            var rootInfo = DirToAssetInfoTree(refPaths);
            var refInfos = FileListToAssetInfos(fileList);

            // 隶属信息
            var spritePackingDict = new Dictionary<string, string>();

            // 根据搜索结果来挂载额外信息
            for (int i = 0; i < fileList.Count; i++)
            {
                for (int j = 0; j < guidList.Count; j++)
                {
                    if (searchRetList[i][j])
                    {
                        AssetInfo info = GenAssetInfo(resFileList[j]);
                        info.isExtra = true;
                        refInfos[i].AddChild(info);

                        // 隶属
                        string val;
                        if (!spritePackingDict.TryGetValue(info.fileRelativePath, out val))
                        {
                            var assetImporter = AssetImporter.GetAtPath(info.fileRelativePath);
                            TextureImporter textureImporter = assetImporter as TextureImporter;
                            if (textureImporter)
                            {
                                val = textureImporter.spritePackingTag;
                            }
                        }

                        info.bindObj = val;
                    }
                }
            }
            MergeAssetInfo(rootInfo, refInfos);

            if (rootInfo.hasChildren)
            {
                data = rootInfo;
            }
            EditorUtility.ClearProgressBar();
        }

        public override void ExportCsv()
        {
            string path = AssetDanshariUtility.GetSaveFilePath(typeof(AssetDependenciesWindow).Name);
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            var style = AssetDanshariStyle.Get();
            var sb = new StringBuilder();
            sb.AppendFormat("\"{0}\",", style.nameHeaderContent.text);
            sb.AppendFormat("\"{0}\"\n", style.dependenciesHeaderContent2.text);

            foreach (var info in data.children)
            {
                ExportCsvDataDir(info, sb, "├");
            }

            AssetDanshariUtility.SaveFileText(path, sb.ToString());
            GUIUtility.ExitGUI();
        }

        private void ExportCsvDataDir(AssetInfo assetInfo, StringBuilder sb, string pre)
        {
            if (assetInfo.isExtra)
            {
                sb.AppendFormat(",\"{0}\"\n", assetInfo.displayName);
            }
            else if (assetInfo.isFolder)
            {
                sb.AppendLine(pre + assetInfo.displayName);
            }
            else
            {
                sb.AppendFormat("\"{0}\"", pre + assetInfo.displayName);
                if (assetInfo.hasChildren && assetInfo.children.Count > 0)
                {
                    sb.AppendFormat(",\"{0}\"", assetInfo.children.Count.ToString());
                }
                sb.AppendLine();
            }

            if (assetInfo.hasChildren)
            {
                foreach (var childInfo in assetInfo.children)
                {
                    ExportCsvDataDir(childInfo, sb, pre + pre);
                }
            }
        }
    }
}