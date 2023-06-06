using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using GameFramework;
using UnityEngine;
using UnityEngine.Networking;


public partial class FileDownloadManager:MonoBehaviour
{
    private void Awake()
    {
        InitDownload();
    }

    private void OnDestroy()
    {
        //ShutdownDownload();
    }

    public void Update()
    {
        FileSizeUpdate();
    }

    public string[] CheckLocalFile(string[] urls,string folderPath)
    {
        List<string> temp = new List<string>();
        foreach (var iURL in urls)
        {
            string localPath = Path.Combine(folderPath,Path.GetFileName(iURL));
            if (!File.Exists(localPath))
            {
                temp.Add(iURL);
            }
        }
        return temp.ToArray();
    }
}