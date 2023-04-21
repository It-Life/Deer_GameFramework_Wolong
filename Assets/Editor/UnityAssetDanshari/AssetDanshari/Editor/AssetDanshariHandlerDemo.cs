using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace AssetDanshari
{
    public class AssetDanshariHandlerDemo
    {
        public AssetDanshariHandlerDemo()
        {
            AssetDanshariHandler.onDependenciesLoadDataMore += OnDependenciesLoadDataMore;
            AssetDanshariHandler.onDependenciesContextDraw += OnDependenciesContextDraw;
        }

        private void OnDependenciesLoadDataMore(string resPath, List<AssetTreeModel.AssetInfo> resInfos, AssetTreeModel treeModel)
        {
            // 去代码定义文件去查找
            if (resPath != "\"Assets/Simple UI/PNG\"")
            {
                return;
            }

            int preLen = resPath.Length - "PNG".Length - 2;
            string codePath = "Assets/Demo/UISpriteDefine.cs";
            try
            {
                string text = File.ReadAllText(codePath);

                foreach (var assetInfo in resInfos)
                {
                    string searchText = assetInfo.fileRelativePath.Substring(preLen);
                    searchText = searchText.Remove(searchText.Length - 4);

                    if (text.Contains(searchText))
                    {
                        AssetTreeModel.AssetInfo info = treeModel.GenAssetInfo(codePath);
                        info.isExtra = true;
                        assetInfo.AddChild(info);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
            }
        }

        private void OnDependenciesContextDraw(GenericMenu menu)
        {
            menu.AddSeparator(String.Empty);
            menu.AddItem(new GUIContent("生成代码定义/C# 定义"), false, OnDependenciesContextGenCodeCSharp);
            menu.AddItem(new GUIContent("生成代码定义/Lua 定义"), false, OnDependenciesContextGenCodeLua);
        }

        private void OnDependenciesContextGenCodeCSharp()
        {

        }

        private void OnDependenciesContextGenCodeLua()
        {

        }
    }
}