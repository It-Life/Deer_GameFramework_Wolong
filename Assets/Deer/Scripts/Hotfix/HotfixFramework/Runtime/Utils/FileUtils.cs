// ================================================
//描 述:
//作 者:杜鑫
//创建时间:2022-06-18 10-48-49
//修改作者:杜鑫
//修改时间:2022-06-18 10-48-49
//版 本:0.1 
// ===============================================
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace HotfixFramework.Runtime 
{
    /// <summary>
    /// Please modify the description.
    /// </summary>
    public static class FileUtils
    {
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
    }
}
