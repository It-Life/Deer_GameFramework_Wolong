// ================================================
//描 述:
//作 者:杜鑫
//创建时间:2022-06-17 16-34-33
//修改作者:杜鑫
//修改时间:2022-06-17 16-34-33
//版 本:0.1 
// ===============================================
using ConfigData;
using Deer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

/// <summary>
/// Please modify the description.
/// </summary>
public class UIFormsConfigDataInfo : ConfigBase<UIFormsConfigDataInfo>
{
    public override string Name => "UIForm_Config";
    public UIForm_Config_Data data;
    private Dictionary<uint, UIForm_Config> m_Infos = new Dictionary<uint, UIForm_Config>();

    public override void Clear()
    {
        data = null;
    }

    public override IEnumerator LoadConfig(bool isReadWritePath)
    {
        AnalyseConfig(Name, isReadWritePath, delegate (byte[] tempData)
        {
            data = ProtobufUtils.Deserialize<UIForm_Config_Data>(tempData);
            if (data != null)
            {
                foreach (var t in data.Items)
                {
                    m_Infos.Add(t.Id, t);
                }
            }
            else
            {
                Log.Error("loadconfig data is null");
            }
        });
        yield return null;
    }
    public UIForm_Config GetConfigById(uint soundId)
    {
        UIForm_Config config;
        if (m_Infos.TryGetValue(soundId, out config))
        {
            return config;
        }

        return null;
    }
}