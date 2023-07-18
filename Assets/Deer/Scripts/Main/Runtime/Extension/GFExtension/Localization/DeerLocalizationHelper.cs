// ================================================
//描 述:
//作 者:txm
//创建时间:2023-07-13 16-43-22
//修改作者:txm
//修改时间:2023-07-13 16-43-22
//版 本:0.1 
// ===============================================
using System;
using System.Collections;
using System.Collections.Generic;
using GameFramework.Localization;
using UnityEngine;
using UnityGameFramework.Runtime;
using CatJson;

public struct LanguageData
{
    public string Key;
    public string Value;
}
/// <summary>
/// 本地化辅助器。
/// </summary>
public class DeerLocalizationHelper : DefaultLocalizationHelper
{

    /// <summary>
    /// 解析字典。
    /// </summary>
    /// <param name="localizationManager">本地化管理器。</param>
    /// <param name="dictionaryString">要解析的字典字符串。</param>
    /// <param name="userData">用户自定义数据。</param>
    /// <returns>是否解析字典成功。</returns>
    public override bool ParseData(ILocalizationManager localizationManager, string dictionaryString, object userData)
    {
        try
        {
            Log.Info(GameEntryMain.Localization.Language);
            Log.Info(dictionaryString);
            JsonParser parser = new JsonParser();
            Dictionary<Language, Dictionary<int, string>> dic = parser.ParseJson<Dictionary<Language, Dictionary<int, string>>>(dictionaryString);
            if (dic.TryGetValue(GameEntryMain.Localization.Language, out Dictionary<int, string> languageDic))
            {
                foreach (var item in languageDic)
                {
                    localizationManager.AddRawString(item.Key.ToString(), item.Value);
                }
                return true;
            }
            else
            {
                Log.Warning("Can not find {0} language config  ", GameEntryMain.Localization.Language);
                return false;
            }
        }
        catch (Exception exception)
        {
            Log.Warning("Can not parse dictionary string with exception '{0}'.", exception);
            return false;
        }
    }
}