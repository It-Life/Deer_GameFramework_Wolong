// ================================================
//描 述:
//作 者:杜鑫
//创建时间:2022-06-17 15-50-10
//修改作者:杜鑫
//修改时间:2022-06-17 15-50-10
//版 本:0.1 
// ===============================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProcedureEnum
{
    ProcedureMain,
    ProcedureArCatch,
    ProcedureBattle,

	ProcedureGamePlay
}

public static partial class Constant 
{
    public static class Scene 
    {
        public static Dictionary<string, string> Scenes = new Dictionary<string,  string>() {
            {ProcedureEnum.ProcedureMain.ToString(),"HotfixBusiness.Procedure.ProcedureMain&Main"},
            {ProcedureEnum.ProcedureBattle.ToString(),"HotfixBusiness.Procedure.ProcedureBattle&PVE_AR"},
            {ProcedureEnum.ProcedureArCatch.ToString(),"HotfixBusiness.Procedure.ProcedureArCatch&FightingScene_AR"},

            {ProcedureEnum.ProcedureGamePlay.ToString(),"HotfixBusiness.Procedure.ProcedureGamePlay&RaceScene_"},
        };

        public static string GetProcedureName(ProcedureEnum procedureEnum)
        {
            return GetProcedureName(procedureEnum.ToString());
        }
        public static string GetProcedureName(string procedureEnumName)
        {
            string procedureAndScene = string.Empty;
            if (Scenes.TryGetValue(procedureEnumName,out procedureAndScene))
            {
                string[] strs = procedureAndScene.Split('&');
                if (strs.Length > 0)
                {
                    return strs[0];
                }
                else
                {
                    Logger.Error($"In GetProcedureName method, Scenes dic 中{procedureEnumName.ToString()}对应的value值格式有问题，请查找是否存在【&】符号连接！");
                }
            }
            else
            {
                Logger.Error($"In GetProcedureName method, Scenes dic 中{procedureEnumName.ToString()}对应的key值不存在！");
            }

            return string.Empty;
        }
        public static string GetSceneName(string procedureEnumName)
        {
            string procedureAndScene = string.Empty;
            if (Scenes.TryGetValue(procedureEnumName,out procedureAndScene))
            {
                string[] strs = procedureAndScene.Split('&');
                if (strs.Length > 0)
                {
                    return strs[1];
                }
                else
                {
                    Logger.Error($"In GetSceneName method, Scenes dic 中{procedureEnumName.ToString()}对应的value值格式有问题，请查找是否存在【&】符号连接！");
                }
            }
            else
            {
                Logger.Error($" In GetSceneName method, Scenes dic 中{procedureEnumName.ToString()}对应的key值不存在！");
            }
            return string.Empty;
        }
        public static string GetSceneNameByProcedureName(string procedureName)
        {
            string procedureAndScene = string.Empty;
            foreach (var scene in Scenes)
            {
                procedureAndScene = scene.Value;
                string[] strs = procedureAndScene.Split('&');
                if (strs.Length > 0)
                {
                    if (strs[0].Equals(procedureName))
                    {
                        return strs[1];
                    }
                }
                else
                {
                    Logger.Error($"In GetSceneName method, Scenes dic 中{procedureName}对应的value值格式有问题，请查找是否存在【&】符号连接！");
                }
            }
            return string.Empty;
        }
    }

}


