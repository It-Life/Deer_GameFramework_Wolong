// ================================================
//描 述 :  
//作 者 : 杜鑫 
//创建时间 : 2021-07-13 23-39-43  
//修改作者 : 杜鑫 
//修改时间 : 2021-07-13 23-39-43  
//版 本 : 0.1 
// ===============================================
using GameFramework;
using GameFramework.ObjectPool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HotfixFramework.Runtime
{
    public class AssetBundleInstanceObject : ObjectBase
    {
        private object m_AssetObject;

        public AssetBundleInstanceObject()
        {
            m_AssetObject = null;
        }


        public static AssetBundleInstanceObject Create(string name, object unityObjectAsset, object uiFormInstance)
        {
            if (unityObjectAsset == null)
            {
                throw new GameFrameworkException("Asset is invalid.");
            }

            AssetBundleInstanceObject unityAssetObject = ReferencePool.Acquire<AssetBundleInstanceObject>();
            unityAssetObject.Initialize(name, uiFormInstance);
            unityAssetObject.m_AssetObject = unityObjectAsset;
            return unityAssetObject;
        }

        public override void Clear()
        {
            base.Clear();
            m_AssetObject = null;
        }

        protected override void Release(bool isShutdown)
        {
            Object.Destroy((Object)Target);
        }
    }
}