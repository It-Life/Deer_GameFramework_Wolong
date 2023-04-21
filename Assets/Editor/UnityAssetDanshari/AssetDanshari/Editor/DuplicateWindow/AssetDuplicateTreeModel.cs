using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace AssetDanshari
{
    public class AssetDuplicateTreeModel : AssetTreeModel
    {
        public class FileMd5Info
        {
            public string filePath;
            public string fileLength;
            public string fileTime;
            public long fileSize;
            public string md5;
        }

        public override void SetDataPaths(string refPathStr, string pathStr, string commonPathStr)
        {
            base.SetDataPaths(refPathStr, pathStr, commonPathStr);
            var style = AssetDanshariStyle.Get();

            var resFileList = GetResFileList();
            var fileList = GetFileMd5Infos(resFileList);
            if (fileList == null || fileList.Count == 0)
            {
                return;
            }

            var rootInfo = new AssetInfo(GetAutoId(), String.Empty, String.Empty);
            var groups = fileList.GroupBy(info => info.md5).Where(g => g.Count() > 1);
            foreach (var group in groups)
            {
                AssetInfo dirInfo = new AssetInfo(GetAutoId(), String.Empty, String.Format(style.duplicateGroup, group.Count()));
                dirInfo.isExtra = true;
                rootInfo.AddChild(dirInfo);

                foreach (var member in group)
                {
                    dirInfo.AddChild(GetAssetInfoByFileMd5Info(member));
                }
            }

            if (rootInfo.hasChildren)
            {
                data = rootInfo;
            }
            EditorUtility.ClearProgressBar();
        }

        private List<FileMd5Info> GetFileMd5Infos(List<string> fileArray)
        {
            var style = AssetDanshariStyle.Get();
            var fileList = new List<FileMd5Info>();

            for (int i = 0; i < fileArray.Count;)
            {
                string file = fileArray[i];
                if (string.IsNullOrEmpty(file))
                {
                    i++;
                    continue;
                }
                EditorUtility.DisplayProgressBar(style.progressTitle, file, i * 1f / fileArray.Count);
                try
                {
                    using (var md5 = MD5.Create())
                    {
                        FileInfo fileInfo = new FileInfo(file);
                        using (var stream = File.OpenRead(fileInfo.FullName))
                        {
                            FileMd5Info info = new FileMd5Info();
                            info.filePath = fileInfo.FullName;
                            info.fileSize = fileInfo.Length;
                            info.fileTime = fileInfo.CreationTime.ToString("yyyy-MM-dd HH:mm:ss");
                            info.md5 = BitConverter.ToString(md5.ComputeHash(stream)).ToLower();
                            fileList.Add(info);
                        }
                    }

                    i++;
                }
                catch (Exception e)
                {
                    if (!EditorUtility.DisplayDialog(style.errorTitle, file + "\n" + e.Message,
                        style.continueStr, style.cancelStr))
                    {
                        EditorUtility.ClearProgressBar();
                        return null;
                    }
                }
            }
            return fileList;
        }

        private AssetInfo GetAssetInfoByFileMd5Info(FileMd5Info fileInfo)
        {
            AssetInfo info = GenAssetInfo(FullPathToRelative(fileInfo.filePath));
            info.bindObj = fileInfo;

            if (fileInfo.fileSize >= (1 << 20))
            {
                fileInfo.fileLength = string.Format("{0:F} MB", fileInfo.fileSize / 1024f / 1024f);
            }
            else if (fileInfo.fileSize >= (1 << 10))
            {
                fileInfo.fileLength = string.Format("{0:F} KB", fileInfo.fileSize / 1024f);
            }
            else
            {
                fileInfo.fileLength = string.Format("{0:F} B", fileInfo.fileSize);
            }

            return info;
        }

        /// <summary>
        /// 去引用到的目录查找所有用到的guid，批量更改
        /// </summary>
        /// <param name="group"></param>
        /// <param name="useInfo"></param>
        public void SetUseThis(AssetInfo group, AssetInfo useInfo)
        {
            var style = AssetDanshariStyle.Get();
            if (!EditorUtility.DisplayDialog(String.Empty, style.sureStr + style.duplicateContextOnlyUseThis.text,
                style.sureStr, style.cancelStr))
            {
                return;
            }

            List<string> patterns = new List<string>();
            foreach (var info in group.children)
            {
                if (info != useInfo)
                {
                    patterns.Add(AssetDatabase.AssetPathToGUID(info.fileRelativePath));
                }
            }

            string replaceStr = AssetDatabase.AssetPathToGUID(useInfo.fileRelativePath);
            List<string> fileList = GetRefFileList();

            ThreadDoFilesTextSearchReplace(fileList, patterns, replaceStr, GetSearchResultList(fileList.Count, 0));
            EditorUtility.DisplayProgressBar(style.progressTitle, style.deleteFile, 0.98f);
            SetRemoveAllOther(group, useInfo);
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog(String.Empty, style.progressFinish, style.sureStr);
        }

        private void SetRemoveAllOther(AssetInfo group, AssetInfo selectInfo)
        {
            foreach (var info in group.children)
            {
                if (info != selectInfo && !info.deleted)
                {
                    if (AssetDatabase.DeleteAsset(info.fileRelativePath))
                    {
                        info.deleted = true;
                    }
                }
            }
        }

        /// <summary>
        /// 手动添加数据
        /// </summary>
        /// <param name="filePaths"></param>
        public int AddManualData(string[] filePaths)
        {
            var fileList = GetFileMd5Infos(filePaths.ToList());
            if (fileList == null || fileList.Count < 2 || !HasData())
            {
                EditorUtility.ClearProgressBar();
                return 0;
            }

            var style = AssetDanshariStyle.Get();
            AssetInfo dirInfo = new AssetInfo(GetAutoId(), String.Empty, String.Format(style.duplicateGroup, fileList.Count));
            dirInfo.isExtra = true;
            data.AddChild(dirInfo);

            foreach (var member in fileList)
            {
                dirInfo.AddChild(GetAssetInfoByFileMd5Info(member));
            }
            EditorUtility.ClearProgressBar();
            return dirInfo.children[dirInfo.children.Count - 1].id;
        }

        public override void ExportCsv()
        {
            string path = AssetDanshariUtility.GetSaveFilePath(typeof(AssetDuplicateWindow).Name);
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            var style = AssetDanshariStyle.Get();
            var sb = new StringBuilder();
            sb.AppendFormat("\"{0}\",", style.duplicateHeaderContent.text);
            sb.AppendFormat("\"{0}\",", style.duplicateHeaderContent2.text);
            sb.AppendFormat("\"{0}\",", style.duplicateHeaderContent3.text);
            sb.AppendFormat("\"{0}\"\n", style.duplicateHeaderContent4.text);

            foreach (var group in data.children)
            {
                sb.AppendLine(String.Format(style.duplicateGroup, group.displayName));

                foreach (var info in group.children)
                {
                    sb.AppendFormat("\"{0}\",", info.displayName);
                    sb.AppendFormat("\"{0}\",", info.fileRelativePath);

                    FileMd5Info md5Info = info.bindObj as FileMd5Info;
                    sb.AppendFormat("\"{0}\",", md5Info.fileLength);
                    sb.AppendFormat("\"{0}\"\n", md5Info.fileTime);
                }
            }

            AssetDanshariUtility.SaveFileText(path, sb.ToString());
            GUIUtility.ExitGUI();
        }
    }
}