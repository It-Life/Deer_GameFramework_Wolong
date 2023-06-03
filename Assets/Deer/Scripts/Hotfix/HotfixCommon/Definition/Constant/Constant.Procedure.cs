
using System.Collections.Generic;


/// <summary>
/// 游戏常量类
/// </summary>
public static partial class Constant
{
    public class ProcedureInfo
    {
        /// <summary>
        /// 流程名字
        /// </summary>
        public string ProcedureName { get; }

        public bool IsCheckAsset { get; }
        public bool IsJumpScene { get; }

        public string GroupName { get; }

        public string SceneName { get; }

        public ProcedureInfo(string procedureName, bool isCheck,bool isJumpScene, string groupName, string sceneName)
        {
            ProcedureName = procedureName;
            IsCheckAsset = isCheck;
            IsJumpScene = isJumpScene;
            GroupName = groupName;
            SceneName = sceneName;            
        }
    }

    /// <summary>
    /// 游戏流程
    /// </summary>
    public static class Procedure
    {
        
        public const string ProcedureMainMenu = "HotfixBusiness.Procedure.ProcedureMainMenu";
        public const string ProcedureResetMain = "HotfixBusiness.Procedure.ProcedureResetMain";
        
        public const string ProcedureADeerExample = "HotfixADeerExample.Procedure.ProcedureADeerExample";
        public const string ProcedureDeerMain = "HotfixADeerExample.Procedure.ProcedureDeerMain";
        public const string ProcedureDeerLogin = "HotfixADeerExample.Procedure.ProcedureDeerLogin";
        
        public const string ProcedureAGameExample = "HotfixAGameExample.Procedure.ProcedureAGameExample";
        public const string ProcedureGamePlay = "HotfixAGameExample.Procedure.ProcedureGamePlay";
        public const string ProcedureGameMenu = "HotfixAGameExample.Procedure.ProcedureGameMenu";
        private static Dictionary<string, ProcedureInfo> ProcedureInfos = new Dictionary<string, ProcedureInfo>()
        {
            {ProcedureMainMenu,new ProcedureInfo(ProcedureMainMenu,false,false,"BaseAssets","")},
            {ProcedureADeerExample,new ProcedureInfo(ProcedureADeerExample,true,false,"ADeerExample","")},
            {ProcedureDeerLogin,new ProcedureInfo(ProcedureDeerLogin,false,true,"ADeerExample","")},
            {ProcedureDeerMain,new ProcedureInfo(ProcedureDeerMain,false,true,"ADeerExample","Main")},
            {ProcedureAGameExample,new ProcedureInfo(ProcedureAGameExample,true,false,"AGameExample","")},
            {ProcedureGamePlay,new ProcedureInfo(ProcedureGamePlay,false,true,"AGameExample","RaceScene_")},
            {ProcedureGameMenu,new ProcedureInfo(ProcedureGameMenu,false,false,"AGameExample","")},
        };

        public static bool IsJumpScene(string procedureName)
        {
            if (ProcedureInfos.ContainsKey(procedureName))
            {
                var  info = ProcedureInfos[procedureName];
                return info.IsJumpScene;
            }
            return false;
        }
        public static bool IsCheckAsset(string procedureName)
        {
            if (ProcedureInfos.ContainsKey(procedureName))
            {
                var  info = ProcedureInfos[procedureName];
                return info.IsCheckAsset;
            }
            return false;
        }
        public static string FindAssetGroup(string procedureName)
        {
            if (ProcedureInfos.ContainsKey(procedureName))
            {
                var  info = ProcedureInfos[procedureName];
                return info.GroupName;
            }
            return string.Empty;
        }
        public static string FindSceneName(string procedureName)
        {
            if (ProcedureInfos.ContainsKey(procedureName))
            {
                var info = ProcedureInfos[procedureName];
                return info.SceneName;
            }
            return string.Empty;
        }
    }
}
