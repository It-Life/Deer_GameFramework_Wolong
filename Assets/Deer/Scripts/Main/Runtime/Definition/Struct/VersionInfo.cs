// ================================================
//描 述 :  
//作 者 : 杜鑫 
//创建时间 : 2021-07-11 17-04-12  
//修改作者 : 杜鑫 
//修改时间 : 2021-07-11 17-04-12  
//版 本 : 0.1 
// ===============================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Main.Runtime
{
    public class VersionInfo
    {
        public bool ForceUpdateGame;

        public string LatestGameVersion;

        public int InternalGameVersion;

        public int InternalResourceVersion;

        public string GameUpdateUrl;

        public int VersionListLength;


        public int VersionListHashCode;


        public int VersionListZipLength;


        public int VersionListZipHashCode;
    }
}