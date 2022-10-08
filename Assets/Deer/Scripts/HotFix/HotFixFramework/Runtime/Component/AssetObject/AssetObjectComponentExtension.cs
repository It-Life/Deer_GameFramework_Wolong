// ================================================
//描 述:
//作 者:杜鑫
//创建时间:2022-06-09 14-19-43
//修改作者:杜鑫
//修改时间:2022-06-09 14-19-43
//版 本:0.1 
// ===============================================
using Deer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HotfoxFramework.Runtime 
{
	/// <summary>
	/// Please modify the description.
	/// </summary>
	public static class AssetObjectComponentExtension
	{
		public static void LoadGameObject(this AssetObjectComponent assetObjectComponent, int nLoadSerial, string strPath, string strShowName, LoadAssetObjectComplete loadAssetObjectComplete = null)
		{
			assetObjectComponent.LoadAssetAsync(nLoadSerial, strPath, strShowName, typeof(GameObject), loadAssetObjectComplete);
		}
		public static void LoadTexture2D(this AssetObjectComponent assetObjectComponent, int nLoadSerial, string strPath, string strShowName, LoadAssetObjectComplete loadAssetObjectComplete = null)
		{
			assetObjectComponent.LoadAssetAsync(nLoadSerial, strPath, strShowName, typeof(Texture2D), loadAssetObjectComplete);
		}
		public static void LoadAnimatorControllerCollection(this AssetObjectComponent assetObjectComponent, int nLoadSerial, string strPath, string strShowName, LoadAssetObjectComplete loadAssetObjectComplete = null)
		{
			//assetObjectComponent.LoadAssetAsync(nLoadSerial, strPath, strShowName, typeof(AnimatorControllerCollection), loadAssetObjectComplete);
		}
	}
}