using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GameFramework;
using UnityEditor;
using UnityEngine;


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
                    Log.Error("文件夹不存在 Path=" + dir);
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
                    Log.Error("文件夹不存在 Path=" + dir);
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
                    Log.Error("文件夹不存在 Path=" + dir);
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

    /// <summary>
    /// C# 侧使用的文件流
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="isReadWritePath"></param>
    /// <param name="action"></param>
    public static void FileReadAllBytes(string filePath, bool isReadWritePath, Action<bool, byte[]> action)
    {
        if (isReadWritePath)
        {
            filePath = Path.Combine(GameEntry.Resource.ReadWritePath, filePath);
            if (!File.Exists(filePath))
            {
                Log.Error("filepath:" + filePath + " not exists");
                return;
            }
            byte[] result = File.ReadAllBytes(filePath);
            action?.Invoke(true, result);
        }
        else
        {
            filePath = GetStreamingAssetsPlatformPathUrl(filePath);
            GameEntry.Config.ReadConfigWithStreamingAssets(filePath, delegate (bool isRead, byte[] result) {
                action?.Invoke(isRead, result);
            });
        }
    }
    public static string GetStreamingAssetsPlatformPathUrl(string filePath) 
    {
        filePath =
#if UNITY_ANDROID && !UNITY_EDITOR
            Application.streamingAssetsPath + "/" + filePath;
#elif UNITY_IPHONE && !UNITY_EDITOR
            "file://" + Application.streamingAssetsPath + "/" + filePath;
#elif UNITY_STANDLONE_WIN || UNITY_EDITOR
            "file://" + Application.streamingAssetsPath + "/" + filePath;
#else
            string.Empty;
#endif
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

    public static bool CanConfigReadWritePath() 
    {
        if (GameEntry.Resource.ResourceMode == GameFramework.Resource.ResourceMode.Package)
        {
            return false;
        }
        return false;
    }

    public static string GetPath(string path)
    {
        return path.Replace("\\","/");
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
            Log.Error("to md5 fail,error:" + ex.Message);
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
}
