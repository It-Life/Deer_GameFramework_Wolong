#if ENABLE_HYBRID_CLR_UNITY
using System;
using System.Collections.Generic;
using System.IO;
using CatJson;
using GameFramework;
using UnityEngine;
using UnityEditor;
using HybridCLR.Editor;
using HybridCLR.Editor.Commands;
using Main.Runtime;

/// <summary>
/// 复制程序集文件
/// </summary>
public static class CopyAssemblies
{
    /// <summary>
    /// 复制热更程序集
    /// </summary>
    public static void DoCopyHotfixAssemblies(BuildTarget buildTarget)
    {
        string targetPath = $"{DeerSettingsUtils.HotfixAssemblyTextAssetPath()}";
        // 清空热更程序集文件夹
        FolderUtils.ClearFolder(targetPath);
        // 检查文件夹是否存在
        if (!Directory.Exists(targetPath))
        {
            Directory.CreateDirectory(targetPath);
        }
        // 复制热更程序集到资源文件夹
        foreach (var dll in SettingsUtil.HotUpdateAssemblyFilesIncludePreserved)
        {
            foreach (var hotUpdateAssembly in DeerSettingsUtils.DeerHybridCLRSettings.HotUpdateAssemblies)
            {
                if (dll == hotUpdateAssembly.Assembly)
                {
                    string dllPath = $"{SettingsUtil.GetHotUpdateDllsOutputDirByTarget(buildTarget)}/{dll}";
                    /*using FileStream fileStream = new FileStream(dllPath, FileMode.Open, FileAccess.Read);
                    int hashCode = Utility.Verifier.GetCrc32(fileStream);*/
                    string dllBytesPath = Path.Combine(targetPath, $"{dll}{DeerSettingsUtils.DeerHybridCLRSettings.AssemblyAssetExtension}");
                    File.Copy(dllPath, dllBytesPath, true);
                    byte[] bytes = File.ReadAllBytes(dllBytesPath);
                    int length = bytes.Length;
                    int hashCode = Utility.Verifier.GetCrc32(bytes);
                    bytes = Utility.Compression.Compress(bytes);
                    File.WriteAllBytes(dllBytesPath, bytes);
                    int compressedLength = bytes.Length;
                    int compressedHashCode = Utility.Verifier.GetCrc32(bytes);
                    string dllBytesNewPath = Path.Combine(targetPath, $"{dll}.{hashCode}{DeerSettingsUtils.DeerHybridCLRSettings.AssemblyAssetExtension}");
                    File.Move(dllBytesPath, dllBytesNewPath);
                    //FileInfo fileInfo = new FileInfo(dllPath);
                    //long size = (long)Math.Ceiling(fileInfo.Length / 1024f);
                    m_ListAssemblies.Add(new AssemblyInfo(dll,"Hotfix",
                        hotUpdateAssembly.AssetGroupName,hashCode, length,compressedHashCode, compressedLength));
                    break;  
                }
            }
        }

        //设置热更程序集
        DeerSettingsUtils.SetHybridCLRHotUpdateAssemblies(SettingsUtil.HotUpdateAssemblyFilesIncludePreserved);

        // 刷新资源
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 复制AOT程序集
    /// </summary>
    /// <param name="buildTarget"></param>
    public static void DoCopyAOTAssemblies(BuildTarget buildTarget)
    {
        //获取所有的AOT程序集
        AOTMetaAssembliesHelper.FindAllAOTMetaAssemblies(buildTarget);

        // 清空AOT文件夹
        FolderUtils.ClearFolder(DeerSettingsUtils.AOTAssemblyTextAssetPath);

        //判断AOT文件夹是否存在
        if (!Directory.Exists(DeerSettingsUtils.AOTAssemblyTextAssetPath))
        {
            Directory.CreateDirectory(DeerSettingsUtils.AOTAssemblyTextAssetPath);
        }

        string mergedFileName = AssemblyFileData.GetMergedFileName();
        string mergedFilePath = $"{DeerSettingsUtils.AOTAssemblyTextAssetPath}/{mergedFileName}{DeerSettingsUtils.DeerHybridCLRSettings.AssemblyAssetExtension}";
        using StreamWriter mergedFileWriter = new StreamWriter(mergedFilePath);
        long currentPosition = 0;
        m_ListAssemblyFileData.Clear();
        // 复制AOT程序集到资源文件夹
        foreach (var dll in DeerSettingsUtils.DeerHybridCLRSettings.AOTMetaAssemblies)
        {
            string dllPath = $"{SettingsUtil.GetAssembliesPostIl2CppStripDir(buildTarget)}/{dll}";
            if (!File.Exists(dllPath))
            {
                Debug.LogError($"ab中添加AOT补充元数据dll:{dllPath} 时发生错误,文件不存在。裁剪后的AOT dll在BuildPlayer时才能生成，因此需要你先构建一次游戏App后再打包。");
                continue;
            }
            using StreamReader fileReader = new StreamReader(dllPath);
            long startPosition = currentPosition;
            fileReader.BaseStream.CopyTo(mergedFileWriter.BaseStream);
            long endPosition = currentPosition + fileReader.BaseStream.Length - 1;
            currentPosition = endPosition + 1;
            AssemblyFileData fileData = new AssemblyFileData(dll, startPosition, endPosition);
            m_ListAssemblyFileData.Add(fileData);
        }
        mergedFileWriter.Close();
        mergedFileWriter.Dispose();
        //压缩
        byte[] bytes = File.ReadAllBytes(mergedFilePath);
        int length = bytes.Length;
        //using FileStream fileStream = new FileStream(mergedFilePath, FileMode.Open, FileAccess.Read);
        int hashCode = Utility.Verifier.GetCrc32(bytes);
        bytes = Utility.Compression.Compress(bytes);
        File.WriteAllBytes(mergedFilePath, bytes);
        int compressedLength = bytes.Length;
        int compressedHashCode = Utility.Verifier.GetCrc32(bytes);
        //fileStream.Close();
        //fileStream.Dispose();
        string mergedNewFilePath = $"{DeerSettingsUtils.AOTAssemblyTextAssetPath}/{mergedFileName}.{hashCode}{DeerSettingsUtils.DeerHybridCLRSettings.AssemblyAssetExtension}";
        File.Move(mergedFilePath, mergedNewFilePath);
        //FileInfo fileInfo = new FileInfo(mergedNewFilePath);
        //long size = (long)Math.Ceiling(fileInfo.Length / 1024f);
        m_ListAssemblies.Add(new AssemblyInfo(mergedFileName,"AOT",
            DeerSettingsUtils.DeerGlobalSettings.BaseAssetsRootName,hashCode, length,compressedHashCode, compressedLength));
        // 刷新资源
        AssetDatabase.Refresh();
    }
    private static List<AssemblyInfo> m_ListAssemblies = new List<AssemblyInfo>();
    private static List<AssemblyFileData> m_ListAssemblyFileData = new List<AssemblyFileData>();

    private static byte[] m_test;
    private static byte[] m_test1;
    
    static bool AreArraysEqual(byte[] array1, byte[] array2)
    {
        if (array1.Length != array2.Length)
            return false;

        for (int i = 0; i < array1.Length; i++)
        {
            if (array1[i] != array2[i])
                return false;
        }

        return true;
    }
    /// <summary>
    /// 复制所有程序集
    /// </summary>
    /// <param name="buildTarget"></param>
    public static void DoCopyAllAssemblies(BuildTarget buildTarget)
    {
        m_ListAssemblies.Clear();
        CompileDllCommand.CompileDll(buildTarget);
        RefreshCompressionHelper();
        DoCopyAOTAssemblies(buildTarget);
        DoCopyHotfixAssemblies(buildTarget);
        CreateAssembliesVersion();
    }

    private static void CreateAssembliesVersion()
    {
        string assembly = m_ListAssemblies.ToJson();
        string assemblyFileData = m_ListAssemblyFileData.ToJson();
        assembly = $"{assembly}{AssemblyFileData.GetSeparator()}{assemblyFileData}";
        string path = $"{DeerSettingsUtils.HybridCLRAssemblyPath}/{DeerSettingsUtils.DeerHybridCLRSettings.AssembliesVersionTextFileName}";
        File.WriteAllText(path, assembly);
        string[] ssss = assembly.Split(AssemblyFileData.GetSeparator());
        var m_UpdateAssemblies = ssss[0].ParseJson<List<AssemblyInfo>>();
        var m_UpdateAs = ssss[1].ParseJson<List<AssemblyFileData>>();
    }
    private static bool RefreshCompressionHelper()
    {
        bool retVal = false;
        if (!string.IsNullOrEmpty(DeerSettingsUtils.DeerHybridCLRSettings.CompressionHelperTypeName))
        {
            System.Type compressionHelperType = Utility.Assembly.GetType(DeerSettingsUtils.DeerHybridCLRSettings.CompressionHelperTypeName);
            if (compressionHelperType != null)
            {
                Utility.Compression.ICompressionHelper compressionHelper = (Utility.Compression.ICompressionHelper)Activator.CreateInstance(compressionHelperType);
                if (compressionHelper != null)
                {
                    Utility.Compression.SetCompressionHelper(compressionHelper);
                    return true;
                }
            }
        }
        else
        {
            retVal = true;
        }
        Utility.Compression.SetCompressionHelper(null);
        return retVal;
    }
}
#endif