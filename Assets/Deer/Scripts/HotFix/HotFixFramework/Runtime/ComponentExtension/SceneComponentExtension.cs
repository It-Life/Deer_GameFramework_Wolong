// ================================================
//描 述 :  
//作 者 : 杜鑫 
//创建时间 : 2021-08-13 23-09-24  
//修改作者 : 杜鑫 
//修改时间 : 2021-08-13 23-09-24  
//版 本 : 0.1 
// ===============================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Deer
{
    public static class SceneComponentExtension
    {
        public static void LoadSceneEx(this SceneComponent sceneComponent,string sceneName, int priority, object userData) 
        {
            string sceneFullName = AssetUtility.Scene.GetTempSceneAsset(sceneName);
            sceneComponent.LoadScene(sceneFullName, priority, userData);
        }
    }
}