// ================================================
//描 述:
//作 者:AlanDu
//创建时间:2023-02-09 17-30-49
//修改作者:AlanDu
//修改时间:2023-02-09 17-30-49
//版 本:0.1 
// ===============================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using GameFramework;
using GameFramework.Event;
using GameFramework.ObjectPool;
using Main.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityGameFramework.Runtime;
using Object = UnityEngine.Object;

namespace HotfixFramework.Runtime 
{
	public class DownloadInfo
	{
		public string name;
		public BundleDownInfo BundleDownInfo;
		public DownloadInfo(string name,BundleDownInfo bundleDownInfo = null)
		{
			this.name = name;
			this.BundleDownInfo = bundleDownInfo;
		}
	}

	public class BundleDownInfo:IReference
	{
		public string downUrl;
		public string localUrl;
		public object userData;
		public Action<object, object> successAction;
		public Action<object> failureAction;
		public void Clear()
		{
			downUrl = null;
			userData = null;
			successAction = null;
			failureAction = null;
		}
	}

	/// <summary>
	/// Please modify the description.
	/// </summary>
	public class RemoteLoadBundleComponent : GameFrameworkComponent
	{
		[SerializeField]
		private Transform m_BundleInstanceRoot = null;
		public Transform BundleInstanceRoot
		{
			get { return m_BundleInstanceRoot; }
		}
		[SerializeField]
		private Transform m_BundleGroupInstanceRoot = null;

		public Transform BundleGroupInstanceRoot
		{
			get { return m_BundleGroupInstanceRoot; }
		}
		private int curDownLoadId = -1;
		private Dictionary<string, Object> dicLoadeds = new Dictionary<string, Object>();
		private Dictionary<int, object> m_AssetObjectToLoad = new Dictionary<int, object>();
		private List<string> listLoadingUrl = new List<string>();

		private IObjectPool<AssetBundleInstanceObject> m_InstancePool; //AssetObject资源池   
		[SerializeField]
		private float m_InstanceAutoReleaseInterval = 5f;

		[SerializeField]
		private int m_InstanceCapacity = 16;

		[SerializeField]
		private float m_InstanceExpireTime = 60f;

		[SerializeField]
		private int m_InstancePriority = 0;

		private Dictionary<string, string> m_DicServerVersion = new Dictionary<string, string>();

		private static string BundleVersionList = "BundleVersionList.dat";

		private Queue<BundleDownInfo> taskDownInfos = new Queue<BundleDownInfo>();
		private bool m_IsLastLoadFinish = true;

		private Dictionary<string, List<BundleDownInfo>> m_listLoadingFinishDownInfos =
			new Dictionary<string, List<BundleDownInfo>>();
		private Dictionary<string, List<BundleDownInfo>> m_listBundleLoadingFinishInfos =
			new Dictionary<string, List<BundleDownInfo>>();

		private List<BundleDownInfo> m_listObjectLoadings = new List<BundleDownInfo>();
		protected override void Awake()
		{
			base.Awake();
			m_InstancePool = GameEntry.ObjectPool.CreateSingleSpawnObjectPool<AssetBundleInstanceObject>("Asset Bundle Pool", 10, 16, 2, 0);
			m_InstancePool.Priority = m_InstancePriority;
			m_InstancePool.ExpireTime = m_InstanceExpireTime;
			m_InstancePool.Capacity = m_InstanceCapacity;
			m_InstancePool.AutoReleaseInterval = m_InstanceAutoReleaseInterval;
		}
		protected void Start()
		{
			if (m_BundleInstanceRoot == null)
			{
				Log.Error("You must set Bundle instance root first.");
				return;
			}

			if (m_BundleGroupInstanceRoot == null)
			{
				Log.Error("You must set Bundle Group instance root first.");
				return;
			}
			GameEntry.Event.Subscribe(DownloadSuccessEventArgs.EventId, OnLoadAssetSuccess);
			GameEntry.Event.Subscribe(DownloadFailureEventArgs.EventId, OnLoadAssetFailure);
		}
		
		protected void OnDestroy()
		{
			m_InstancePool = null;
			dicLoadeds.Clear();
			listLoadingUrl.Clear();
			m_DicServerVersion.Clear();
			m_DicServerVersion = null;
			dicLoadeds = null;
			listLoadingUrl = null;
		}

		private void Update()
		{
			if (taskDownInfos.Count>0 && m_IsLastLoadFinish)
			{
				StartDownAssetBundle();
			}
		}
		
		public void InitDownloadAssetInfo(string url)
		{
			url = $"{url}{BundleVersionList}";
			string localFilePath = FileUtils.GetAssetBundleLocalPathByUrl(url);
			localFilePath = localFilePath.Replace("//", "/");
			GameEntry.Download.AddDownload(localFilePath,url,new DownloadInfo(BundleVersionList));
		}


		public void ShowAsset(string url,object userData,Action<object,object> successAction = null,Action<object> failureAction = null)
		{
			if (string.IsNullOrEmpty(url))
			{
				Log.Error("下载地址为空");
				failureAction?.Invoke($"下载地址为空");
				return;
			}
			BundleDownInfo bundleDownInfo = ReferencePool.Acquire<BundleDownInfo>();
			bundleDownInfo.downUrl = url;
			//bundleDownInfo.localUrl = FileUtils.GetAssetBundleLocalPathByUrl(url);
			bundleDownInfo.userData = userData;
			bundleDownInfo.successAction = successAction;
			bundleDownInfo.failureAction = failureAction;
			taskDownInfos.Enqueue(bundleDownInfo);
			
		}

		private void StartDownAssetBundle()
		{
			m_IsLastLoadFinish = false;
			BundleDownInfo bundleDownInfo = taskDownInfos.Dequeue();
			m_listObjectLoadings.Add(bundleDownInfo);
			string url = bundleDownInfo.downUrl;
			string localUrl = bundleDownInfo.localUrl;
			if (listLoadingUrl.Contains(localUrl))
			{
				Debug.Log($"资源正在下载 Url:{url}");
				if (m_listLoadingFinishDownInfos.ContainsKey(localUrl))
				{
					m_listLoadingFinishDownInfos[localUrl].Add(bundleDownInfo);
				}
				else
				{
					m_listLoadingFinishDownInfos[localUrl] = new List<BundleDownInfo>();
					m_listLoadingFinishDownInfos[localUrl].Add(bundleDownInfo);
				}
				m_IsLastLoadFinish = true;
				return;
			}
			GameEntry.UI.OpenUILoadingOneForm();
			if (IsShowObject(bundleDownInfo))
			{
				return;
			}
			listLoadingUrl.Add(localUrl);
			m_IsLastLoadFinish = true;
			StartDownLoadAssetBundle(bundleDownInfo, OnDownLoadSuccess, OnDownLoadFailure);
		}

		private bool IsShowObject(BundleDownInfo bundleDownInfo)
		{
			string localUrl = bundleDownInfo.localUrl;
			AssetBundleInstanceObject assetObj = m_InstancePool.Spawn(localUrl);
			if (assetObj != null)
			{
				ShowAssetObj(bundleDownInfo,assetObj.Target);
				return true;
			}
			if (dicLoadeds.ContainsKey(localUrl))
			{
				Object asset = dicLoadeds[localUrl];
				AssetBundleInstanceObject assetObject = AssetBundleInstanceObject.Create(bundleDownInfo.localUrl, asset,Instantiate(asset));
				m_InstancePool.Register(assetObject, true);
				ShowAssetObj(bundleDownInfo,assetObject.Target);
				return true;
			}
			return false;
		}

		private void OnDownLoadSuccess(BundleDownInfo bundleDownInfo)
		{
			string localUrl = bundleDownInfo.localUrl;
			listLoadingUrl.Remove(localUrl);
			if (IsShowObject(bundleDownInfo))
			{
				return;
			}
			if (m_listBundleLoadingFinishInfos.ContainsKey(localUrl))
			{
				m_listBundleLoadingFinishInfos[localUrl].Add(bundleDownInfo);
				Debug.Log($"资源正在加载 Url:{localUrl}");
				return;
			}
			else
			{
				m_listBundleLoadingFinishInfos[localUrl] = new List<BundleDownInfo>();
			}
			StartCoroutine(IELoadBundle(bundleDownInfo));
		}

		private void OnDownLoadFailure(BundleDownInfo bundleDownInfo)
		{
			listLoadingUrl.Remove(bundleDownInfo.localUrl);
			bundleDownInfo.failureAction?.Invoke("下载资源失败");
			m_listObjectLoadings.Remove(bundleDownInfo);
			if (m_listObjectLoadings.Count == 0)
			{
				GameEntry.UI.CloseUILoadingOneForm();
			}
			m_IsLastLoadFinish = true;
			ReferencePool.Release(bundleDownInfo);
		}
		private IEnumerator IELoadBundle(BundleDownInfo bundleDownInfo)
		{
			string localUrl = bundleDownInfo.localUrl;
			AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(localUrl);
			yield return request;
			AssetBundle bundle =  request.assetBundle;
			//加载Bundle中某个资源
			Object asset = bundle.LoadAsset(bundle.name);
			AssetBundleInstanceObject assetObject = AssetBundleInstanceObject.Create(localUrl, asset,Instantiate(asset));
			m_InstancePool.Register(assetObject, true);
			dicLoadeds.Add(localUrl,asset);
			ShowAssetObj(bundleDownInfo,assetObject.Target);
			if (m_listBundleLoadingFinishInfos.ContainsKey(localUrl))
			{
				List<BundleDownInfo> _listBundleDownInfos = m_listBundleLoadingFinishInfos[localUrl];
				foreach (var downInfoOne in _listBundleDownInfos)
				{
					IsShowObject(downInfoOne);
				}
				m_listBundleLoadingFinishInfos[localUrl].Clear();
				m_listBundleLoadingFinishInfos[localUrl] = null;
				m_listBundleLoadingFinishInfos.Remove(localUrl);
			}
		}
		public void HideAsset(int nLoadSerial)
		{
			if (m_AssetObjectToLoad.TryGetValue(nLoadSerial, out object obj))
			{
				GameObject tempObj = obj as GameObject;
				if (tempObj != null)
				{
					tempObj.SetActive(false);
				}
				Unspawn(obj);
				m_AssetObjectToLoad.Remove(nLoadSerial);
			}
		}

		public bool IsLoadingAsset(int nLoadSerial)
		{
			/*if (listLoadingUrl.Contains(nLoadSerial))
			{
				return true;
			}

			if (listBundleLoading.Contains(nLoadSerial))
			{
				return true;
			}*/

			return false;
		}

		private void ShowAssetObj(BundleDownInfo bundleDownInfo, object asset)
		{
			object userData = bundleDownInfo.userData;
			EntityData entityData = userData as  EntityData;
			int nLoadSerial = entityData.Id;
			GameObject obj = asset as GameObject;
			obj.SetActive(true);
			Transform trans = obj.GetComponent<Transform>();
			trans.SetParent(m_BundleInstanceRoot);
			m_AssetObjectToLoad.Add(nLoadSerial,asset);
			bundleDownInfo.successAction?.Invoke(asset,userData);
			m_listObjectLoadings.Remove(bundleDownInfo);
			if (m_listObjectLoadings.Count == 0)
			{
				GameEntry.UI.CloseUILoadingOneForm();
			}
			m_IsLastLoadFinish = true;
			ReferencePool.Release(bundleDownInfo);
		}
		/// <summary>
		/// 回收资源
		/// </summary>
		/// <param name="asset"></param>
		private void Unspawn(object asset)
		{
			if (m_InstancePool == null)
			{
				Log.Error("AssetBundleComponent Unspwn m_InstancePool null");
				return;
			}
			m_InstancePool.Unspawn(asset);
		}
		private void StartDownLoadAssetBundle(BundleDownInfo bundleDownInfo,Action<BundleDownInfo> downLoadSuccessAction,Action<BundleDownInfo> downLoadFailureAction)
		{
			string downUrl = bundleDownInfo.downUrl;
			string localFilePath = bundleDownInfo.localUrl;
			if (!FileUtils.ExistsFile(localFilePath))
			{
				Log.Info("资源开始下载 " + Time.time + " 资源远端地址  " + downUrl);
				curDownLoadId = GameEntry.Download.AddDownload(localFilePath, downUrl,new DownloadInfo(localFilePath,bundleDownInfo));
			}else
			{
				string localFileMd5 = FileUtils.Md5ByPathName(localFilePath);
				try
				{
					string serverMd5 = string.Empty;
					m_DicServerVersion.TryGetValue(Path.GetFileName(downUrl), out serverMd5);
					if (localFileMd5 == serverMd5)
					{
						downLoadSuccessAction?.Invoke(bundleDownInfo);
					}
					else
					{
						Log.Info("资源开始下载 " + Time.time + " 资源下载地址  " + downUrl);
						curDownLoadId = GameEntry.Download.AddDownload(localFilePath, downUrl,new DownloadInfo(localFilePath,bundleDownInfo));
					}
				}
				catch (Exception e)
				{
					Debug.LogError(e);
					downLoadFailureAction.Invoke(bundleDownInfo);
					throw;
				}
			}
		}

		private void OnLoadAssetSuccess(object sender, GameEventArgs e)
		{
			DownloadSuccessEventArgs a = (DownloadSuccessEventArgs)e;
			if (!(a.UserData is DownloadInfo downloadInfo))
			{
				return;
			}
			if (downloadInfo.name == BundleVersionList)
			{
				string[] lines = File.ReadAllLines(a.DownloadPath);
				foreach (var line in lines)
				{
					string[] infos = line.Split(":");
					if (infos.Length > 1)
					{
						m_DicServerVersion.Add(infos[0],infos[1]);
					}
				}
			}
			else
			{
				Log.Info("资源下载成功 " + Time.time + " 资源下载地址 " + a.DownloadUri);
				BundleDownInfo bundleDownInfo = downloadInfo.BundleDownInfo;
				OnDownLoadSuccess(bundleDownInfo);
				List<BundleDownInfo> _listBundleDownInfos;
				if (m_listLoadingFinishDownInfos.TryGetValue(bundleDownInfo.localUrl,out _listBundleDownInfos))
				{
					if (_listBundleDownInfos != null && _listBundleDownInfos.Count > 0)
					{
						foreach (var downInfoOne in _listBundleDownInfos)
						{
							OnDownLoadSuccess(downInfoOne);
						}
					}
					m_listLoadingFinishDownInfos.Remove(bundleDownInfo.localUrl);
				}
			}
		}
		private void OnLoadAssetFailure(object sender, GameEventArgs e)
		{
			DownloadFailureEventArgs a = (DownloadFailureEventArgs)e;
			if (!(a.UserData is DownloadInfo downloadInfo))
			{
				return;
			}
			if (downloadInfo.name != BundleVersionList)
			{
				Log.Info("资源下载失败 " + Time.time + " 资源下载地址 " + a.DownloadUri);
				BundleDownInfo bundleDownInfo = downloadInfo.BundleDownInfo;
				OnDownLoadFailure(bundleDownInfo);
			}
		}
	}
}
