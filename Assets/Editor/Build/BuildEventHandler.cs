// ================================================
//描 述 :  
//作 者 : 杜鑫 
//创建时间 : 2021-10-06 16-05-46  
//修改作者 : 杜鑫 
//修改时间 : 2021-10-06 16-05-46  
//版 本 : 0.1 
// ===============================================

using System.Collections.Generic;
using System.IO;
using GameFramework;
using Main.Runtime;
using UGFExtensions.Editor.ResourceTools;
using UnityEditor;
using UnityEngine;
using UnityGameFramework.Editor.ResourceTools;

public class BuildEventHandler : IBuildEventHandler
{
    public bool ContinueOnFailure => false;

    private readonly string CommitResourcesPath = Application.dataPath + $"/../CommitResources/{DeerSettingsUtils.ResourcesArea.ResAdminType}{DeerSettingsUtils.ResourcesArea.ResAdminCode}/";
    private readonly List<string> StreamingAssetsPaths = new List<string>()
    {
        Utility.Path.GetRegularPath(Path.Combine(Application.dataPath, "StreamingAssets", "AssetsHotfix")),
        Utility.Path.GetRegularPath(Path.Combine(Application.dataPath, "StreamingAssets", "AssetsPacked")),
        Utility.Path.GetRegularPath(Path.Combine(Application.dataPath, "StreamingAssets", DeerSettingsUtils.DeerGlobalSettings.ConfigFolderName)),
    };
    private readonly List<string> StreamingAssetsFilePaths = new List<string>()
    {
        Utility.Path.GetRegularPath(Path.Combine(Application.dataPath, "StreamingAssets", "GameFrameworkList.dat")),
        Utility.Path.GetRegularPath(Path.Combine(Application.dataPath, "StreamingAssets", "GameFrameworkVersion.dat")),
    };
    private VersionInfo m_VersionInfo = new VersionInfo();

    /// <summary>
    /// 所有平台生成开始前的预处理事件。
    /// </summary>
    /// <param name="productName">产品名称。</param>
    /// <param name="companyName">公司名称。</param>
    /// <param name="gameIdentifier">游戏识别号。</param>
    /// <param name="gameFrameworkVersion">游戏框架版本。</param>
    /// <param name="unityVersion">Unity 版本。</param>
    /// <param name="applicableGameVersion">适用游戏版本。</param>
    /// <param name="internalResourceVersion">内部资源版本。</param>
    /// <param name="platforms">生成的目标平台。</param>
    /// <param name="assetBundleCompression">AssetBundle 压缩类型。</param>
    /// <param name="compressionHelperTypeName">压缩解压缩辅助器类型名称。</param>
    /// <param name="additionalCompressionSelected">是否进行再压缩以降低传输开销。</param>
    /// <param name="forceRebuildAssetBundleSelected">是否强制重新构建 AssetBundle。</param>
    /// <param name="buildEventHandlerTypeName">生成资源事件处理函数名称。</param>
    /// <param name="outputDirectory">生成目录。</param>
    /// <param name="buildAssetBundleOptions">AssetBundle 生成选项。</param>
    /// <param name="workingPath">生成时的工作路径。</param>
    /// <param name="outputPackageSelected">是否生成单机模式所需的文件。</param>
    /// <param name="outputPackagePath">为单机模式生成的文件存放于此路径。若游戏是单机游戏，生成结束后将此目录中对应平台的文件拷贝至 StreamingAssets 后打包 App 即可。</param>
    /// <param name="outputFullSelected">是否生成可更新模式所需的远程文件。</param>
    /// <param name="outputFullPath">为可更新模式生成的远程文件存放于此路径。若游戏是网络游戏，生成结束后应将此目录上传至 Web 服务器，供玩家下载用。</param>
    /// <param name="outputPackedSelected">是否生成可更新模式所需的本地文件。</param>
    /// <param name="outputPackedPath">为可更新模式生成的本地文件存放于此路径。若游戏是网络游戏，生成结束后将此目录中对应平台的文件拷贝至 StreamingAssets 后打包 App 即可。</param>
    /// <param name="buildReportPath">生成报告路径。</param>
    public void OnPreprocessAllPlatforms(string productName, string companyName, string gameIdentifier, string gameFrameworkVersion, 
        string unityVersion, string applicableGameVersion, int internalResourceVersion, Platform platforms, AssetBundleCompressionType assetBundleCompression, 
        string compressionHelperTypeName, bool additionalCompressionSelected, bool forceRebuildAssetBundleSelected, string buildEventHandlerTypeName, 
        string outputDirectory, BuildAssetBundleOptions buildAssetBundleOptions, string workingPath, bool outputPackageSelected, string outputPackagePath, 
        bool outputFullSelected, string outputFullPath, bool outputPackedSelected, string outputPackedPath, string buildReportPath)
    {
        m_VersionInfo.InternalResourceVersion = internalResourceVersion;
        /*foreach (var item in StreamingAssetsPaths)
        {
            FolderUtils.ClearFolder(item);
        }
        foreach (var item in StreamingAssetsFilePaths)
        {
            if (File.Exists(item))
            {
                File.Delete(item);
            }
        }*/
        FolderUtils.ClearFolder(Application.streamingAssetsPath);
        UGFExtensions.SpriteCollection.SpriteCollectionUtility.RefreshSpriteCollection();
        if (string.IsNullOrEmpty(DeerSettingsUtils.DeerPathConfig.ResourceCollectionPath))
        {
            ResourceRuleEditorUtility.RefreshResourceCollection();
        }
        else
        {
            ResourceRuleEditorUtility.RefreshResourceCollection(DeerSettingsUtils.DeerPathConfig.ResourceCollectionPath);
        }
        BuildEventHandlerLuban.OnPreprocessAllPlatforms(platforms, outputFullSelected);
#if ENABLE_HYBRID_CLR_UNITY
        BuildEventHandlerWolong.OnPreprocessAllPlatforms(platforms);
#endif
    }

    /// <summary>
    /// 某个平台生成开始前的预处理事件。
    /// </summary>
    /// <param name="platform">生成平台。</param>
    /// <param name="workingPath">生成时的工作路径。</param>
    /// <param name="outputPackageSelected">是否生成单机模式所需的文件。</param>
    /// <param name="outputPackagePath">为单机模式生成的文件存放于此路径。若游戏是单机游戏，生成结束后将此目录中对应平台的文件拷贝至 StreamingAssets 后打包 App 即可。</param>
    /// <param name="outputFullSelected">是否生成可更新模式所需的远程文件。</param>
    /// <param name="outputFullPath">为可更新模式生成的远程文件存放于此路径。若游戏是网络游戏，生成结束后应将此目录上传至 Web 服务器，供玩家下载用。</param>
    /// <param name="outputPackedSelected">是否生成可更新模式所需的本地文件。</param>
    /// <param name="outputPackedPath">为可更新模式生成的本地文件存放于此路径。若游戏是网络游戏，生成结束后将此目录中对应平台的文件拷贝至 StreamingAssets 后打包 App 即可。</param>

    public void OnPreprocessPlatform(Platform platform, string workingPath, bool outputPackageSelected,
        string outputPackagePath,
        bool outputFullSelected, string outputFullPath, bool outputPackedSelected, string outputPackedPath)
    {
#if ENABLE_HYBRID_CLR_UNITY
        BuildEventHandlerWolong.OnPreprocessPlatform(platform);
#endif
    }
    /// <summary>
    /// 某个平台生成 AssetBundle 完成事件。
    /// </summary>
    /// <param name="platform">生成平台。</param>
    /// <param name="workingPath">生成时的工作路径。</param>
    /// <param name="outputPackageSelected">是否生成单机模式所需的文件。</param>
    /// <param name="outputPackagePath">为单机模式生成的文件存放于此路径。若游戏是单机游戏，生成结束后将此目录中对应平台的文件拷贝至 StreamingAssets 后打包 App 即可。</param>
    /// <param name="outputFullSelected">是否生成可更新模式所需的远程文件。</param>
    /// <param name="outputFullPath">为可更新模式生成的远程文件存放于此路径。若游戏是网络游戏，生成结束后应将此目录上传至 Web 服务器，供玩家下载用。</param>
    /// <param name="outputPackedSelected">是否生成可更新模式所需的本地文件。</param>
    /// <param name="outputPackedPath">为可更新模式生成的本地文件存放于此路径。若游戏是网络游戏，生成结束后将此目录中对应平台的文件拷贝至 StreamingAssets 后打包 App 即可。</param>
    /// <param name="assetBundleManifest">AssetBundle 的描述文件。</param>
    public void OnBuildAssetBundlesComplete(Platform platform, string workingPath, bool outputPackageSelected,
        string outputPackagePath, bool outputFullSelected, string outputFullPath, bool outputPackedSelected,
        string outputPackedPath, AssetBundleManifest assetBundleManifest){}
    /// <summary>
    /// 某个平台可更新模式版本列表文件的输出事件。
    /// </summary>
    /// <param name="platform">生成平台。</param>
    /// <param name="versionListPath">可更新模式版本列表文件的路径。</param>
    /// <param name="versionListLength">可更新模式版本列表文件的长度。</param>
    /// <param name="versionListHashCode">可更新模式版本列表文件的校验值。</param>
    /// <param name="versionListZipLength">可更新模式版本列表文件压缩后的长度。</param>
    /// <param name="versionListZipHashCode">可更新模式版本列表文件压缩后的校验值。</param>
    public void OnOutputUpdatableVersionListData(Platform platform, string versionListPath, int versionListLength,
        int versionListHashCode, int versionListZipLength, int versionListZipHashCode)
    {
        m_VersionInfo.VersionListLength = versionListLength;
        m_VersionInfo.VersionListHashCode = versionListHashCode;
        m_VersionInfo.VersionListZipLength = versionListZipLength;
        m_VersionInfo.VersionListZipHashCode = versionListZipHashCode;
    }
    /// <summary>
    /// 所有平台生成结束后的后处理事件。
    /// </summary>
    /// <param name="productName">产品名称。</param>
    /// <param name="companyName">公司名称。</param>
    /// <param name="gameIdentifier">游戏识别号。</param>
    /// <param name="gameFrameworkVersion">游戏框架版本。</param>
    /// <param name="unityVersion">Unity 版本。</param>
    /// <param name="applicableGameVersion">适用游戏版本。</param>
    /// <param name="internalResourceVersion">内部资源版本。</param>
    /// <param name="platforms">生成的目标平台。</param>
    /// <param name="assetBundleCompression">AssetBundle 压缩类型。</param>
    /// <param name="compressionHelperTypeName">压缩解压缩辅助器类型名称。</param>
    /// <param name="additionalCompressionSelected">是否进行再压缩以降低传输开销。</param>
    /// <param name="forceRebuildAssetBundleSelected">是否强制重新构建 AssetBundle。</param>
    /// <param name="buildEventHandlerTypeName">生成资源事件处理函数名称。</param>
    /// <param name="outputDirectory">生成目录。</param>
    /// <param name="buildAssetBundleOptions">AssetBundle 生成选项。</param>
    /// <param name="workingPath">生成时的工作路径。</param>
    /// <param name="outputPackageSelected">是否生成单机模式所需的文件。</param>
    /// <param name="outputPackagePath">为单机模式生成的文件存放于此路径。若游戏是单机游戏，生成结束后将此目录中对应平台的文件拷贝至 StreamingAssets 后打包 App 即可。</param>
    /// <param name="outputFullSelected">是否生成可更新模式所需的远程文件。</param>
    /// <param name="outputFullPath">为可更新模式生成的远程文件存放于此路径。若游戏是网络游戏，生成结束后应将此目录上传至 Web 服务器，供玩家下载用。</param>
    /// <param name="outputPackedSelected">是否生成可更新模式所需的本地文件。</param>
    /// <param name="outputPackedPath">为可更新模式生成的本地文件存放于此路径。若游戏是网络游戏，生成结束后将此目录中对应平台的文件拷贝至 StreamingAssets 后打包 App 即可。</param>
    /// <param name="buildReportPath">生成报告路径。</param>
    public void OnPostprocessAllPlatforms(string productName, string companyName, string gameIdentifier, 
        string gameFrameworkVersion, string unityVersion, string applicableGameVersion, 
        int internalResourceVersion, Platform platforms, AssetBundleCompressionType assetBundleCompression, 
        string compressionHelperTypeName, bool additionalCompressionSelected, bool forceRebuildAssetBundleSelected, 
        string buildEventHandlerTypeName, string outputDirectory, BuildAssetBundleOptions buildAssetBundleOptions, 
        string workingPath, bool outputPackageSelected, string outputPackagePath, bool outputFullSelected, string outputFullPath, 
        bool outputPackedSelected, string outputPackedPath, string buildReportPath){}
    /// <summary>
    /// 某个平台生成结束后的后处理事件。
    /// </summary>
    /// <param name="platform">生成平台。</param>
    /// <param name="workingPath">生成时的工作路径。</param>
    /// <param name="outputPackageSelected">是否生成单机模式所需的文件。</param>
    /// <param name="outputPackagePath">为单机模式生成的文件存放于此路径。若游戏是单机游戏，生成结束后将此目录中对应平台的文件拷贝至 StreamingAssets 后打包 App 即可。</param>
    /// <param name="outputFullSelected">是否生成可更新模式所需的远程文件。</param>
    /// <param name="outputFullPath">为可更新模式生成的远程文件存放于此路径。若游戏是网络游戏，生成结束后应将此目录上传至 Web 服务器，供玩家下载用。</param>
    /// <param name="outputPackedSelected">是否生成可更新模式所需的本地文件。</param>
    /// <param name="outputPackedPath">为可更新模式生成的本地文件存放于此路径。若游戏是网络游戏，生成结束后将此目录中对应平台的文件拷贝至 StreamingAssets 后打包 App 即可。</param>
    /// <param name="isSuccess">是否生成成功。</param>
    public void OnPostprocessPlatform(Platform platform, string workingPath, bool outputPackageSelected, string outputPackagePath,
        bool outputFullSelected, string outputFullPath, bool outputPackedSelected, string outputPackedPath, bool isSuccess)
    {
        /*foreach (var item in StreamingAssetsPaths)
        {
            if (!Directory.Exists(item))
            {
                Directory.CreateDirectory(item);
            }
        }*/
        bool isCopyOther = false;
        bool isOpenCommitResourcesPath = false;
        bool isOpenFullPath = false;
        if (outputPackageSelected)
        {
            if (FolderUtils.CopyFilesToRootPath(outputPackagePath, Application.streamingAssetsPath, SearchOption.AllDirectories))
            {
                Debug.Log("拷贝单机资源文件成功！");
            }

            isCopyOther = true;
        }
        if (!outputPackageSelected && outputPackedSelected)
        {
            if (FolderUtils.CopyFilesToRootPath(outputPackedPath, Application.streamingAssetsPath, SearchOption.AllDirectories))
            {
                Debug.Log("拷贝包体资源文件成功！");
            }
            isCopyOther = true;
        }
        //更新包文件
        if (outputFullSelected)
        {
            m_VersionInfo.ForceUpdateGame = false;
            m_VersionInfo.GameUpdateUrl = "";
            m_VersionInfo.LatestGameVersion = "";
            string versionInfoJson = JsonUtility.ToJson(m_VersionInfo);
            FileUtils.CreateFile(Path.Combine(outputFullPath,DeerSettingsUtils.DeerGlobalSettings.ResourceVersionFileName),versionInfoJson);
            if (DeerSettingsUtils.ResourcesArea.WhetherCopyResToCommitPath)
            {
                string commitPath = CommitResourcesPath + "/" + platform;
                if (!Directory.Exists(commitPath))
                {
                    Directory.CreateDirectory(commitPath);
                }
                if (DeerSettingsUtils.ResourcesArea.CleanCommitPathRes)
                {
                    FolderUtils.ClearFolder(commitPath);
                }
                List<string> commitAssetsPaths = new List<string>()
                {
                    Utility.Path.GetRegularPath(Path.Combine(commitPath, "AssetsHotfix")),
                    Utility.Path.GetRegularPath(Path.Combine(commitPath, "AssetsPacked")),
                };
                foreach (var item in commitAssetsPaths)
                {
                    FolderUtils.ClearFolder(item);
                }
                List<string> files = new List<string>(Directory.GetFiles(commitPath));
                foreach (var file in files)
                {
                    if (file.Contains("GameFrameworkVersion"))
                    {
                        if (File.Exists(file))
                        {
                            File.Delete(file);
                            break;
                        }
                    }
                }
                if (FolderUtils.CopyFilesToRootPath(outputFullPath, commitPath, SearchOption.AllDirectories))
                {
                    Debug.Log("更新资源文件拷贝完毕！");
                }

                isCopyOther = true;
                isOpenCommitResourcesPath = true;
            }
            else
            {
                isOpenFullPath = true;
            }

        }
        if (isCopyOther)
        {
#if ENABLE_HYBRID_CLR_UNITY
            BuildEventHandlerWolong.OnPostprocessPlatform(platform, outputPackageSelected, outputFullSelected, outputPackedSelected, CommitResourcesPath);
#endif
            BuildEventHandlerLuban.OnPostprocessPlatform(platform, outputPackageSelected, outputFullSelected, outputPackedSelected, CommitResourcesPath);
            AssetDatabase.Refresh();
        }
        if (isOpenCommitResourcesPath)
        {
            Application.OpenURL("file://"+ CommitResourcesPath);
        }
        if (isOpenFullPath)
        {
            Application.OpenURL("file://"+ outputFullPath);
        }
    }
}
