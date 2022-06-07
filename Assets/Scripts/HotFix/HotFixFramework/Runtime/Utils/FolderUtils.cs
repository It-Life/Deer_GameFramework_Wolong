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
}
