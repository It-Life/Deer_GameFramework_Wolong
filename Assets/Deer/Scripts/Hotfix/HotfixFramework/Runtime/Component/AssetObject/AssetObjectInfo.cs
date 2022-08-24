// ================================================
//描 述 :  
//作 者 : 杜鑫 
//创建时间 : 2022-03-09 19-37-25  
//修改作者 : 杜鑫 
//修改时间 : 2022-03-09 19-37-25  
//版 本 : 0.1 
// ===============================================
using GameFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Deer
{
	public class AssetObjectInfo: IReference
	{
        private int m_SerialId;
        private string m_AssetObjectName;

        public AssetObjectInfo()
        {
            m_SerialId = 0;
            m_AssetObjectName = null;
        }

        public string ShowAssetObjectName
        {
            get;
            private set;
        }
        /// <summary>
        /// 获取UI序列号
        /// </summary>
        public int SerialId
        {
            get
            {
                return m_SerialId;
            }
        }

        /// <summary>
        /// 获取UI界面资源名称
        /// </summary>
        public string AssetObjectName
        {
            get
            {
                return m_AssetObjectName;
            }
        }


        public static AssetObjectInfo Create(int serialId, string assetName, string showName)
        {
            AssetObjectInfo assetObjectInfo = ReferencePool.Acquire<AssetObjectInfo>();
            assetObjectInfo.m_SerialId = serialId;
            assetObjectInfo.m_AssetObjectName = assetName;
            assetObjectInfo.ShowAssetObjectName = showName;
            return assetObjectInfo;
        }

        public void Clear()
        {
            m_SerialId = 0;
            m_AssetObjectName = null;
        }
    }
}