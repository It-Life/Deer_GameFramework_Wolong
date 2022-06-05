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
namespace Deer
{
    public class AssetInstanceObject : ObjectBase
    {
        private object m_AssetObject;

        public AssetInstanceObject()
        {
            m_AssetObject = null;
        }


        public static AssetInstanceObject Create(string name, object unityObjectAsset, object uiFormInstance)
        {
            if (unityObjectAsset == null)
            {
                throw new GameFrameworkException("Asset is invalid.");
            }

            AssetInstanceObject unityAssetObject = ReferencePool.Acquire<AssetInstanceObject>();
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
            GameEntry.Resource.UnloadAsset(m_AssetObject);
            UnityEngine.GameObject.Destroy((Object)Target);
        }
    }
}