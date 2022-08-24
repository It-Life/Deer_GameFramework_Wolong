using System.Collections;
using System.Collections.Generic;
using GameFramework.Config;
using GameFramework;
using UnityEngine;
public class GMNetWindow : DeerGMNetWindowHelper
{
    // private Dictionary<int, Sounds_Config> m_GMConfigs;
    private Vector2 m_LogScrollPosition;
    private Vector2 m_LogScrollPosition2;
    private Vector2 m_StackScrollPosition;
    private string m_SendCommand;
    // private GMConfigInfo m_CurrentConfig;

    public GMNetWindow()
    {
        //m_GMConfigs = 
        //DataConfigGMTableInfo.Singleton().GetGMConfigList();
        //if (m_GMConfigs == null)
        //{
        //    Log.Error("GM工具配置表错误");
        //}
    }
    public void OnDrawScrollableWindow()
    {
        //int nLength = m_GMConfigs.Count;
        int nIndex = 0;
        GUILayout.Label("<b>GMNetTools</b>");
        GUILayout.BeginVertical("box");
        {
            m_LogScrollPosition = GUILayout.BeginScrollView(m_LogScrollPosition);
            {
                nIndex = 0;
                //foreach (var item in m_GMConfigs)
                //{
                //    if (nIndex % 2 == 0)
                //    {
                //        GUILayout.BeginHorizontal();
                //    }
                //    nIndex += 1;
                //    if (GUILayout.Button(item.Value.Name + "  \r\n" + item.Value.Desc, GUILayout.Height(50f), GUILayout.Width(300)))
                //    {
                //        m_SendCommand = item.Value.Code;
                //        m_SendCommand = m_SendCommand.Replace("|", ",");
                //        m_SendCommand = m_SendCommand.Replace('#', '"');
                //        m_CurrentConfig = item.Value;
                //    }
                //    if (nIndex % 2 == 0)
                //    {
                //        GUILayout.EndHorizontal();
                //    }
                //}
                if (nIndex % 2 != 0)
                {
                    GUILayout.EndHorizontal();
                }

            }
            GUILayout.EndScrollView();
        }
        GUILayout.EndVertical();

        m_StackScrollPosition = Vector2.zero;
        GUILayout.BeginVertical("box");
        {
            m_StackScrollPosition = GUILayout.BeginScrollView(m_StackScrollPosition, GUILayout.Height(40f));
            {
                GUILayout.BeginHorizontal("box");
                {
                    if (GUILayout.Button("发送指令", GUILayout.Height(30f)))
                    {
                        ReqGmMessage();
                    }
                    m_SendCommand = GUILayout.TextField(m_SendCommand, GUILayout.Height(30f));
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
        }
        GUILayout.EndVertical();
    }

    private void ReqGmMessage()
    {
        if (string.IsNullOrEmpty(m_SendCommand))
        {
            return;
        }
        //if (m_CurrentConfig != null && m_CurrentConfig.Side == 1)    //前端自己处理
        //{
        //    if (m_CurrentConfig.Code.StartsWith("Trigger.Instruction"))   //触发新手引导
        //    {
        //        BGEvent.SendEvent(GameEvent.SysEvent.EVENT_SYS_GM_TRIGGER, m_SendCommand);
        //    }
        //}
        //else                           //服务器处理
        //{
        //    LuaUtil.CallMethod("LuaUtil", "CallSendGMMsg", m_SendCommand);
        //}
    }
}

