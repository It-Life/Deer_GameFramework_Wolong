using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FolderUtils
{
	/// <summary>
	/// delete folder,
	/// </summary>
	/// <param name="path"></param>
	/// <param name="safeDelete"></param>
	/// <returns></returns>
	public static bool ClearFolder(string path)
	{
		var di = new DirectoryInfo(path);
		if (!di.Exists) return false;
		foreach (var file in di.GetFiles())
		{
			file.Delete();
		}
		foreach (var dir in di.GetDirectories())
		{
			dir.Delete(true);
		}
		return true;
	}
	public static bool CopyFiles(string sourceRootPath, string destRootPath, SearchOption searchOption = SearchOption.AllDirectories) 
	{
		string[] fileNames = Directory.GetFiles(sourceRootPath, "*", searchOption);
		foreach (string fileName in fileNames)
		{
			string destFileName = Path.Combine(destRootPath, fileName.Substring(sourceRootPath.Length));
			FileInfo destFileInfo = new FileInfo(destFileName);
			if (destFileInfo.Directory != null && !destFileInfo.Directory.Exists)
			{
				destFileInfo.Directory.Create();
			}
			File.Copy(fileName, destFileName, true);
		}
		return true;
	}
}
