using System;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace AssetDanshari
{
    public class AssetDependenciesTreeModel : AssetTreeModel
    {
        public override void SetDataPaths(string refPathStr, string pathStr, string commonPathStr)
        {
            base.SetDataPaths(refPathStr, pathStr, commonPathStr);

            var resFileList = GetResFileList();
            var guidList = GetGuidFromFileList(resFileList);
            var fileList = GetRefFileList();
            var searchRetList = GetSearchResultList(fileList.Count, guidList.Count);

            ThreadDoFilesTextSearchReplace(fileList, guidList, String.Empty, searchRetList);
            var rootInfo = DirToAssetInfoTree(resPaths);
            var resInfos = FileListToAssetInfos(resFileList);

            // 根据搜索结果来挂载额外信息
            for (int i = 0; i < fileList.Count; i++)
            {
                for (int j = 0; j < guidList.Count; j++)
                {
                    if (searchRetList[i][j])
                    {
                        AssetInfo info = GenAssetInfo(fileList[i]);
                        info.isExtra = true;
                        resInfos[j].AddChild(info);
                    }
                }
            }

            if (AssetDanshariHandler.onDependenciesLoadDataMore != null)
            {
                AssetDanshariHandler.onDependenciesLoadDataMore(pathStr, resInfos, this);
            }

            MergeAssetInfo(rootInfo, resInfos);

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