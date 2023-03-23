using System.Collections.Generic;
using GameFramework;
using GameFramework.Event;
using UnityEngine;
using UnityGameFramework.Runtime;
using WebRequestFailureEventArgs = UnityGameFramework.Runtime.WebRequestFailureEventArgs;
using WebRequestSuccessEventArgs = UnityGameFramework.Runtime.WebRequestSuccessEventArgs;

namespace UGFExtensions.Texture
{
    public partial class TextureSetComponent
    {
        private WebRequestComponent m_WebRequestComponent;
        private List<int> m_AllRemoveNetworkRequest;
        private void InitializedWeb()
        {
            m_AllRemoveNetworkRequest = new List<int>();
            m_WebRequestComponent = UnityGameFramework.Runtime.GameEntry.GetComponent<WebRequestComponent>();
            EventComponent eventComponent = UnityGameFramework.Runtime.GameEntry.GetComponent<EventComponent>();
            eventComponent.Subscribe(WebRequestSuccessEventArgs.EventId,OnWebGetTextureSuccess);
            eventComponent.Subscribe(WebRequestFailureEventArgs.EventId,OnWebGetTextureFailure);
        }
        /// <summary>
        /// 通过网络设置图片
        /// </summary>
        /// <param name="setTexture2dObject">需要设置图片的对象</param>
        /// <param name="saveFilePath">保存网络图片到本地的路径</param>
        public int SetTextureByNetwork(ISetTexture2dObject setTexture2dObject, string saveFilePath = null)
        {
            if (m_TexturePool.CanSpawn(setTexture2dObject.Texture2dFilePath))
            {
                var texture = (Texture2D)m_TexturePool.Spawn(setTexture2dObject.Texture2dFilePath).Target;
                SetTexture(setTexture2dObject, texture);
            }
            else
            {
                return m_WebRequestComponent.AddWebRequest(setTexture2dObject.Texture2dFilePath, WebGetTextureData.Create(setTexture2dObject,this,saveFilePath));
            }

            return 0;
        }

        public void RemoveWebRequest(int serialId)
        {
            m_AllRemoveNetworkRequest.Add(serialId);
            m_WebRequestComponent.RemoveWebRequest(serialId);
        }
        
        private void OnWebGetTextureFailure(object sender, GameEventArgs e)
        {
            WebRequestFailureEventArgs webRequestSuccessEventArgs = (WebRequestFailureEventArgs)e;
            WebGetTextureData webGetTextureData = webRequestSuccessEventArgs.UserData as WebGetTextureData;
            if (webGetTextureData == null || webGetTextureData.UserData != this)
            {
                return;
            }
            Log.Error("Can not download Texture2D from '{0}' with error message '{1}'.",webRequestSuccessEventArgs.WebRequestUri,webRequestSuccessEventArgs.ErrorMessage);
            ReferencePool.Release(webGetTextureData);
        }

        private void OnWebGetTextureSuccess(object sender, GameEventArgs e)
        {
            WebRequestSuccessEventArgs webRequestSuccessEventArgs = (WebRequestSuccessEventArgs)e;
            WebGetTextureData webGetTextureData = webRequestSuccessEventArgs.UserData as WebGetTextureData;
            if (webGetTextureData == null || webGetTextureData.UserData != this)
            {
                return;
            }
            Texture2D tex = new Texture2D(0, 0, TextureFormat.RGBA32, false);
            var bytes = webRequestSuccessEventArgs.GetWebResponseBytes();
            tex.LoadImage(bytes);
            if (!string.IsNullOrEmpty(webGetTextureData.FilePath))
            {
                SaveTexture(webGetTextureData.FilePath, bytes);
            }
            m_TexturePool.Register(TextureItemObject.Create(webGetTextureData.SetTexture2dObject.Texture2dFilePath, tex, TextureLoad.FromNet), true);
            if (!m_AllRemoveNetworkRequest.Contains(webRequestSuccessEventArgs.Id))
            {
                SetTexture(webGetTextureData.SetTexture2dObject, tex);
            }
            else
            {
                m_AllRemoveNetworkRequest.Remove(webRequestSuccessEventArgs.Id);
            }
            ReferencePool.Release(webGetTextureData);
        }
      
    }
}