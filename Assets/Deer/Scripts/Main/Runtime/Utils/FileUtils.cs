using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using GameFramework;
using GameFramework.Resource;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Utility = GameFramework.Utility;

namespace Main.Runtime 
{
    public enum UnityPlatformPathType:int
    {
        dataPath = 0,
        streamingAssetsPath,
        persistentDataPath,
        temporaryCachePath,
    }
    public class FileUtils
    {
        public static bool CreateFile(string filePath, bool isCreateDir = true)
        {
            if (!File.Exists(filePath))
            {
                string dir = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(dir))
                {
                    if (isCreateDir)
                    {
                        Directory.CreateDirectory(dir);
                    }
                    else
                    {
                        Logger.Error("文件夹不存在 Path=" + dir);
                        return false;
                    }
                }
                File.Create(filePath);
            }
            return true;
        }

        public static bool CreateFile(string filePath, string info, bool isCreateDir = true)
        {
            StreamWriter sw;//流信息
            FileInfo t = new FileInfo(filePath);
            if (!t.Exists)
            {//判断文件是否存在
                string dir = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(dir))
                {
                    if (isCreateDir)
                    {
                        Directory.CreateDirectory(dir);
                    }
                    else
                    {
#if UNITY_EDITOR
                        EditorUtility.DisplayDialog("Tips", "文件夹不存在", "CANCEL");
#endif
                        Logger.Error("文件夹不存在 Path=" + dir);
                        return false;
                    }
                }
                sw = t.CreateText();//不存在，创建
            }
            else
            {
                sw = t.AppendText();//存在，则打开
            }
            sw.WriteLine(info);//以行的形式写入信息
            sw.Close();//关闭流
            sw.Dispose();//销毁流
            return true;
        }
        public static bool CreateFileByByte(string filePath, byte[] info, bool isCreateDir = true)
        {
            FileInfo t = new FileInfo(filePath);
            if (!t.Exists)
            {//判断文件是否存在
                string dir = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(dir))
                {
                    if (isCreateDir)
                    {
                        Directory.CreateDirectory(dir);
                    }
                    else
                    {
#if UNITY_EDITOR
                        EditorUtility.DisplayDialog("Tips", "文件夹不存在", "CANCEL");
#endif
                        Logger.Error("文件夹不存在 Path=" + dir);
                        return false;
                    }
                }
            }
            File.WriteAllBytes(t.FullName, info);
            return true;
        }
        /// <summary>
        /// 查找文件
        /// </summary>
        /// <param name="path">文件夹路径</param>
        /// <param name="subfolder">是否查找子文件</param>
        /// <returns>文件路径集合</returns>
        public static List<string> FindFiles(string path, bool subfolder = true)
        {
            List<string> fileList = new List<string>();

            if (Directory.Exists(path))
            {
                foreach (string file in Directory.GetFiles(path))
                {
                    fileList.Add(file);
                }
                if (subfolder)
                {
                    foreach (string directory in Directory.GetDirectories(path))
                    {
                        fileList.AddRange(FindFiles(directory, subfolder));
                    }
                }
            }

            return fileList;
        }

        public static bool ExistsFile(string filePath)
        {
            FileInfo t = new FileInfo(filePath);
            if (t.Exists)
            {
                return true;
            }
            return false;
        }

        public static string GetPlatformPath(string filePath, UnityPlatformPathType pathType,bool isUrl = false)
        {
            filePath = string.Empty;
//#if UNITY_ANDROID && !UNITY_EDITOR
            if (pathType == UnityPlatformPathType.dataPath)
            {
                if (isUrl)
                    filePath = Application.streamingAssetsPath + "/" + filePath;
                else
                    filePath = Application.dataPath + "!assets" + "/" + filePath;
            }
            else if (pathType == UnityPlatformPathType.streamingAssetsPath)
            {

            }
            else if (pathType == UnityPlatformPathType.persistentDataPath)
            {

            }
            else if (pathType == UnityPlatformPathType.temporaryCachePath)
            {

            }

            //#elif UNITY_IOS
            filePath = "file://" + Application.streamingAssetsPath + "/" + filePath;
            if (pathType == UnityPlatformPathType.dataPath)
            {
                if (isUrl)
                    filePath = Application.streamingAssetsPath + "/" + filePath;
                else
                    filePath = Application.dataPath + "!assets" + "/" + filePath;
            }
            else if (pathType == UnityPlatformPathType.streamingAssetsPath)
            {

            }
            else if (pathType == UnityPlatformPathType.persistentDataPath)
            {

            }
            else if (pathType == UnityPlatformPathType.temporaryCachePath)
            {

            }
//#else
            if (pathType == UnityPlatformPathType.dataPath)
            {
                if (isUrl)
                    filePath = Application.streamingAssetsPath + "/" + filePath;
                else
                    filePath = Application.dataPath + "!assets" + "/" + filePath;
            }
            else if (pathType == UnityPlatformPathType.streamingAssetsPath)
            {

            }
            else if (pathType == UnityPlatformPathType.persistentDataPath)
            {

            }
            else if (pathType == UnityPlatformPathType.temporaryCachePath)
            {

            }
//
//#endif
            return filePath;
        }
        public static string GetStreamingAssetsPlatformPath(string filePath)
        {
            filePath =
#if UNITY_ANDROID && !UNITY_EDITOR
             Application.dataPath + "!assets" + "/" + filePath;
#else
            Application.streamingAssetsPath + "/" + filePath;
#endif
            return filePath;
        }

        public static string GetPersistentDataPlatformPath(string filePath)
        {
            filePath =
#if UNITY_ANDROID && !UNITY_EDITOR
             Application.dataPath + "!assets" + "/" + filePath;
#else
            Application.streamingAssetsPath + "/" + filePath;
#endif
            return filePath;
        }

        public static string GetPath(string path)
        {
            return path.Replace("\\", "/");
        }
        public static string Md5ByPathName(string pathName)
        {
            try
            {
                FileStream file = new FileStream(pathName, FileMode.Open);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                Logger.Error("to md5 fail,error:" + ex.Message);
                return "Error";
            }
        }
        public static string GetLengthString(long length)
        {
            if (length < 1024)
            {
                return $"{length.ToString()} Bytes";
            }
            if (length < 1024 * 1024)
            {
                return $"{(length / 1024f):F2} KB";
            }
            return length < 1024 * 1024 * 1024 ? $"{(length / 1024f / 1024f):F2} MB" : $"{(length / 1024f / 1024f / 1024f):F2} GB";
        }
        public static string GetByteLengthString(long byteLength)
        {
            if (byteLength < 1024L) // 2 ^ 10
            {
                return Utility.Text.Format("{0} Bytes", byteLength.ToString());
            }

            if (byteLength < 1048576L) // 2 ^ 20
            {
                return Utility.Text.Format("{0} KB", (byteLength / 1024f).ToString("F2"));
            }

            if (byteLength < 1073741824L) // 2 ^ 30
            {
                return Utility.Text.Format("{0} MB", (byteLength / 1048576f).ToString("F2"));
            }

            if (byteLength < 1099511627776L) // 2 ^ 40
            {
                return Utility.Text.Format("{0} GB", (byteLength / 1073741824f).ToString("F2"));
            }

            if (byteLength < 1125899906842624L) // 2 ^ 50
            {
                return Utility.Text.Format("{0} TB", (byteLength / 1099511627776f).ToString("F2"));
            }

            if (byteLength < 1152921504606846976L) // 2 ^ 60
            {
                return Utility.Text.Format("{0} PB", (byteLength / 1125899906842624f).ToString("F2"));
            }

            return Utility.Text.Format("{0} EB", (byteLength / 1152921504606846976f).ToString("F2"));
        }
        public static string BinToUtf8(byte[] total)
        {
            byte[] result = total;
            if (total[0] == 0xef && total[1] == 0xbb && total[2] == 0xbf)
            {
                // utf8文件的前三个字节为特殊占位符，要跳过
                result = new byte[total.Length - 3];
                System.Array.Copy(total, 3, result, 0, total.Length - 3);
            }

            string utf8string = System.Text.Encoding.UTF8.GetString(result);
            return utf8string;
        }
        public static Dictionary<string, ConfigInfo> AnalyConfigXml(string xml,out string version)
        {
            version = string.Empty;
            Dictionary<string, ConfigInfo> _configs = new Dictionary<string, ConfigInfo>();
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.LoadXml(xml);
            }
            catch (Exception ex)
            {
                throw new GameFrameworkException(
                    GameFramework.Utility.Text.Format("解析配置文件出错，请检查！！errormessage:'{0}'", ex.ToString()));
            }

            XmlElement configRoot = doc.DocumentElement;
            XmlNode node = doc.SelectSingleNode("Root");
            if (node == null)
            {
                Logger.Error("Root node is null");
                return _configs;
            }

            if (node.Attributes != null && node.Attributes[0] != null)
            {
                version = node.Attributes[0].Value;
            }
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                if (node.ChildNodes[i] is XmlElement elem && elem.Name.ToLower() == "fileversion")
                {
                    string fileName = elem.GetAttribute("name");
                    string filePath = elem.GetAttribute("file");
                    string fileHashCode = elem.GetAttribute("hashCode");
                    string fileSize = elem.GetAttribute("size");
                    string nameWithoutExtension = elem.GetAttribute("nameWithoutExtension");
                    string extension = elem.GetAttribute("extension");
                    ConfigInfo configInfo = new ConfigInfo()
                    {
                        Name = fileName,
                        Path = filePath,
                        HashCode = fileHashCode,
                        Size = fileSize,
                        NameWithoutExtension = nameWithoutExtension,
                        Extension = extension,                        
                    };

                    if (!_configs.ContainsKey(filePath))
                    {
                        _configs.Add(filePath, configInfo);
                    }
                    else
                    {
                        Logger.Error("config filePath already exists:" + filePath);
                    }
                }
            }
            return _configs;
        }
        public static IEnumerator LoadBytesCo(string fileUri, LoadBytesCallbacks loadBytesCallbacks, object userData)
        {
            bool isError = false;
            byte[] bytes = null;
            string errorMessage = null;
            DateTime startTime = DateTime.UtcNow;

#if UNITY_5_4_OR_NEWER
            UnityWebRequest unityWebRequest = UnityWebRequest.Get(fileUri);
#if UNITY_2017_2_OR_NEWER
            yield return unityWebRequest.SendWebRequest();
#else
            yield return unityWebRequest.Send();
#endif

#if UNITY_2020_2_OR_NEWER
            isError = unityWebRequest.result != UnityWebRequest.Result.Success;
#elif UNITY_2017_1_OR_NEWER
            isError = unityWebRequest.isNetworkError || unityWebRequest.isHttpError;
#else
            isError = unityWebRequest.isError;
#endif
            bytes = unityWebRequest.downloadHandler.data;
            errorMessage = isError ? unityWebRequest.error : null;
            unityWebRequest.Dispose();
#else
            WWW www = new WWW(fileUri);
            yield return www;

            isError = !string.IsNullOrEmpty(www.error);
            bytes = www.bytes;
            errorMessage = www.error;
            www.Dispose();
#endif

            if (!isError)
            {
                float elapseSeconds = (float)(DateTime.UtcNow - startTime).TotalSeconds;
                loadBytesCallbacks.LoadBytesSuccessCallback(fileUri, bytes, elapseSeconds, userData);
            }
            else if (loadBytesCallbacks.LoadBytesFailureCallback != null)
            {
                loadBytesCallbacks.LoadBytesFailureCallback(fileUri, errorMessage, userData);
            }
        }
        
        public static string localFilePath = "/u3dres/xxx";
        public static string GetLocalPathByUri(string uri)
        {
            if (string.IsNullOrEmpty(uri))
                return null;
            string fileName =  uri.Substring(uri.LastIndexOf('/') + 1);
            string localPath = GetLocalFilePath();
            return localPath + "/" + fileName;
        }
        public static string GetLocalFilePath()
        {
            string path = Application.persistentDataPath + localFilePath;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }
        /// <summary>
        /// 根据URL获取本地资源名字
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetAssetBundleNameByUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return null;
            string fileName = url.Substring(url.LastIndexOf('/') + 1);
            return fileName;
        }

        /// <summary>
        /// 根据URL获取本地资源地址
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetAssetBundleLocalPathByUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return null;
            string localPath = GetAssetBundleLocalFilePath();
            return localPath + "/" + GetAssetBundleNameByUrl(url);
        }
        /// <summary>
        /// 获取本地ab包存储地址
        /// </summary>
        /// <returns></returns>
        public static string GetAssetBundleLocalFilePath()
        {
            String platform = "";
#if UNITY_IOS
            platform = "IOS";
#elif UNITY_ANDROID
            platform = "Android";
#endif
            string path = Application.persistentDataPath + localFilePath + "/" + platform;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }
        /// <summary>
        /// 获取本地资源文件名称
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static string GetLocalFileNameWithoutExtension(string uri)
        {
            if (string.IsNullOrEmpty(uri))
            {
                return null;
            }
            return Path.GetFileNameWithoutExtension(GetAssetBundleLocalPathByUrl(uri));
        }
    }
}
