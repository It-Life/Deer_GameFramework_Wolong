// ================================================
//描 述:
//作 者:AlanDu
//创建时间:2023-07-13 11-31-26
//修改作者:AlanDu
//修改时间:2023-07-13 11-31-26
//版 本:0.1 
// ===============================================
using System.Collections.Generic;
using GameFramework.Resource;
using Main.Runtime;

public class AssemblyInfo
{
	public string Name;
	public string PathRoot;
	public string GroupName;
	public int HashCode;
	public long Length;
	public int CompressedHashCode;
	public long CompressedLength;
	public bool IsLoadReadOnly = true;
	public int RetryCount;
	public AssemblyInfo(string name,string pathRoot,string groupName, int hashCode,long length,int compressedHashCode,long compressedLength)
	{
		Name = name;
		PathRoot = pathRoot;
		GroupName = groupName;
		HashCode = hashCode;
		Length = length;
		CompressedHashCode = compressedHashCode;
		CompressedLength = compressedLength;
	}
}

public class AssemblyFileData
{
	public string Name;
	public long StartPosition;
	public long EndPosition;

	public AssemblyFileData(string name, long startPosition, long endPosition)
	{
		Name = name;
		StartPosition = startPosition;
		EndPosition = endPosition;
	}

	public static string GetMergedFileName()
	{
		return "Merged.dll";
	}

	public static string GetSeparator()
	{
		return "\n---------------------\n";
	}
}

/// <summary>
/// 使用可更新模式并更资源程序集完成时的回调函数。
/// </summary>
/// <param name="result">更新资源结果，全部成功为 true，否则为 false。</param>
public delegate void UpdateAssembliesCompleteCallback(string groupName,bool result);
/// <summary>
/// 使用可更新模式并检查资源程序集完成时的回调函数。
/// </summary>
/// <param name="updateCount">可更新的资源数量。</param>
/// <param name="updateTotalLength">可更新的资源总大小。</param>
public delegate void CheckAssembliesCompleteCallback(int updateCount, long updateTotalLength);
public delegate void OnInitAssembliesCompleteCallback();
public delegate void CheckAssembliesVersionListCompleteCallback(CheckVersionListResult result);

public delegate void OnLoadAssembliesCompleteCallback(Dictionary<string,byte[]> loadCompleteAssemblies);
// 自定义比较器
public class AssembliesComparer : IEqualityComparer<AssemblyInfo>
{
	public bool Equals(AssemblyInfo obj1, AssemblyInfo obj2)
	{
		// 在这里定义你对两个对象的比较逻辑
		return obj2 != null && obj1 != null && obj1.Name == obj2.Name && obj1.HashCode == obj2.HashCode;  // 示例：根据名字比较
	}

	public int GetHashCode(AssemblyInfo obj)
	{
		return obj.GetHashCode();
	}
}


/// <summary>
/// 程序集管理器
/// </summary>
public partial class AssembliesManager
{
	public AssembliesManager()
	{
		OnEnterDownload();
	}
	
	private void LoadBytes(string fileUri, LoadBytesCallbacks loadBytesCallbacks, object userData)
	{
		GameEntryMain.Assemblies.StartCoroutine(FileUtils.LoadBytesCo(fileUri, loadBytesCallbacks, userData));
	}
}

