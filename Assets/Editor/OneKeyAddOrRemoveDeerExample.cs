// ================================================
//描 述:
//作 者:AlanDu
//创建时间:2023-04-21 15-54-19
//修改作者:AlanDu
//修改时间:2023-04-21 15-54-19
//版 本:0.1 
// ===============================================
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Main.Runtime;
using UnityEditor;
using UnityEngine;
using UnityGameFramework.Editor;

/// <summary>
/// 一键添加和移除Deer例子
/// </summary>
public static class OneKeyAddOrRemoveDeerExample
{
	private const string EnableEXAMPLE = "UNITY_ENABLE_DEER_EXAMPLE";
	private static readonly BuildTargetGroup[] BuildTargetGroups = new BuildTargetGroup[]
	{
		BuildTargetGroup.Standalone,
		BuildTargetGroup.iOS,
		BuildTargetGroup.Android,
		BuildTargetGroup.WSA,
		BuildTargetGroup.WebGL
	};
	private static Dictionary<string,string> m_DicExamplePaths = new Dictionary<string, string>()
	{
		["Assets/Deer/AssetsHotfix/EntityPrefabs/DeerExample"] = "1",
		["Assets/Deer/AssetsHotfix/EntityResources/DeerExample"] = "1",
		["Assets/Deer/AssetsHotfix/Scenes/DeerExample"] = "1",
		["Assets/Deer/AssetsHotfix/Sound/Music/Game_Music.mp3"] = "1",
		["Assets/Deer/AssetsHotfix/Sound/Music/Menu_Music.mp3"] = "1",
		["Assets/Deer/AssetsHotfix/Sound/UISound/Fx_01_V2.wav"] = "1",
		["Assets/Deer/AssetsHotfix/Sound/UISound/Fx_02_V2.mp3"] = "1",
		["Assets/Deer/AssetsHotfix/UI/UIArt/AtlasCollection/Menu.asset"] = "1",
		["Assets/Deer/AssetsHotfix/UI/UIArt/AtlasCollection/Others.asset"] = "1",
		["Assets/Deer/AssetsHotfix/UI/UIArt/AtlasCollection/Pause.asset"] = "1",
		["Assets/Deer/AssetsHotfix/UI/UIArt/AtlasCollection/SelectCharacter.asset"] = "1",
		["Assets/Deer/AssetsHotfix/UI/UIArt/AtlasCollection/SelectMode.asset"] = "1",
		["Assets/Deer/AssetsHotfix/UI/UIArt/AtlasCollection/SelectRace.asset"] = "1",
		["Assets/Deer/AssetsHotfix/UI/UIArt/AtlasCollection/Settings.asset"] = "1",
		["Assets/Deer/AssetsHotfix/UI/UIArt/AtlasCollection/SettingSubMenu.asset"] = "1",
		["Assets/Deer/AssetsHotfix/UI/UIArt/Texture/ChooseVehicle_Background.png"] = "1",
		["Assets/Deer/AssetsHotfix/UI/UIArt/Texture/HubSettings_Background.png"] = "1",
		["Assets/Deer/AssetsHotfix/UI/UIArt/Texture/MainMenu_Background.png"] = "1",
		["Assets/Deer/AssetsHotfix/UI/UIArt/Texture/SelectRace_Background.png"] = "1",
		["Assets/Deer/AssetsHotfix/UI/UIArt/Texture/Settings_SubMenu_Background.png"] = "1",
		["Assets/Deer/AssetsHotfix/UI/UIArt/UISprites/Menu"] = "1",
		["Assets/Deer/AssetsHotfix/UI/UIArt/UISprites/Others"] = "1",
		["Assets/Deer/AssetsHotfix/UI/UIArt/UISprites/Pause"] = "1",
		["Assets/Deer/AssetsHotfix/UI/UIArt/UISprites/SelectCharacter"] = "1",
		["Assets/Deer/AssetsHotfix/UI/UIArt/UISprites/SelectMode"] = "1",
		["Assets/Deer/AssetsHotfix/UI/UIArt/UISprites/SelectRace"] = "1",
		["Assets/Deer/AssetsHotfix/UI/UIArt/UISprites/Settings"] = "1",
		["Assets/Deer/AssetsHotfix/UI/UIArt/UISprites/SettingSubMenu"] = "1",
		["Assets/Deer/AssetsHotfix/UI/UIForms/UIBag"] = "1",
		["Assets/Deer/AssetsHotfix/UI/UIForms/UICharacterSelection"] = "1",
		["Assets/Deer/AssetsHotfix/UI/UIForms/UIGameMode"] = "1",
		["Assets/Deer/AssetsHotfix/UI/UIForms/UIGamePlay"] = "1",
		["Assets/Deer/AssetsHotfix/UI/UIForms/UIGameSettle"] = "1",
		["Assets/Deer/AssetsHotfix/UI/UIForms/UIGameStop"] = "1",
		["Assets/Deer/AssetsHotfix/UI/UIForms/UIMenu"] = "1",
		["Assets/Deer/AssetsHotfix/UI/UIForms/UIRaceSelection"] = "1",
		["Assets/Deer/AssetsHotfix/UI/UIForms/UISettingAudio"] = "1",
		["Assets/Deer/AssetsHotfix/UI/UIForms/UISettingGameOptions"] = "1",
		["Assets/Deer/AssetsHotfix/UI/UIForms/UISettings"] = "1",
		["Assets/Deer/Atlas/Menu.spriteatlas"] = "1",
		["Assets/Deer/Atlas/Others.spriteatlas"] = "1",
		["Assets/Deer/Atlas/Pause.spriteatlas"] = "1",
		["Assets/Deer/Atlas/SelectCharacter.spriteatlas"] = "1",
		["Assets/Deer/Atlas/SelectMode.spriteatlas"] = "1",
		["Assets/Deer/Atlas/SelectRace.spriteatlas"] = "1",
		["Assets/Deer/Atlas/Settings.spriteatlas"] = "1",
		["Assets/Deer/Atlas/SettingSubMenu.spriteatlas"] = "1",
		//界面脚本
		["Assets/Deer/Scripts/Hotfix/HotfixBusiness/UI/UIBag"] = "1",
		["Assets/Deer/Scripts/Hotfix/HotfixBusiness/UI/UIBagForm"] = "1",
		["Assets/Deer/Scripts/Hotfix/HotfixBusiness/UI/UICharacterSelection"] = "1",
		["Assets/Deer/Scripts/Hotfix/HotfixBusiness/UI/UIGameMode"] = "1",
		["Assets/Deer/Scripts/Hotfix/HotfixBusiness/UI/UIGamePlay"] = "1",
		["Assets/Deer/Scripts/Hotfix/HotfixBusiness/UI/UIGameSettle"] = "1",
		["Assets/Deer/Scripts/Hotfix/HotfixBusiness/UI/UIGameStop"] = "1",
		["Assets/Deer/Scripts/Hotfix/HotfixBusiness/UI/UIMenu"] = "1",
		["Assets/Deer/Scripts/Hotfix/HotfixBusiness/UI/UIRaceSelection"] = "1",
		["Assets/Deer/Scripts/Hotfix/HotfixBusiness/UI/UISettingAudio"] = "1",
		["Assets/Deer/Scripts/Hotfix/HotfixBusiness/UI/UISettingGameOptions"] = "1",
		["Assets/Deer/Scripts/Hotfix/HotfixBusiness/UI/UISettings"] = "1",
		["Assets/Deer/Scripts/Hotfix/HotfixBusiness/UI/BindComponents/UIBagForm.BindComponents.cs"] = "1",
		["Assets/Deer/Scripts/Hotfix/HotfixBusiness/UI/BindComponents/UIBagForm_SubUI.BindComponents.cs"] = "1",
		["Assets/Deer/Scripts/Hotfix/HotfixBusiness/UI/BindComponents/UICharacterSelectionForm.BindComponents.cs"] = "1",
		["Assets/Deer/Scripts/Hotfix/HotfixBusiness/UI/BindComponents/UIGameModeForm.BindComponents.cs"] = "1",
		["Assets/Deer/Scripts/Hotfix/HotfixBusiness/UI/BindComponents/UIGamePlayForm.BindComponents.cs"] = "1",
		["Assets/Deer/Scripts/Hotfix/HotfixBusiness/UI/BindComponents/UIGameSettleForm.BindComponents.cs"] = "1",
		["Assets/Deer/Scripts/Hotfix/HotfixBusiness/UI/BindComponents/UIGameStopForm.BindComponents.cs"] = "1",
		["Assets/Deer/Scripts/Hotfix/HotfixBusiness/UI/BindComponents/UIMenuForm.BindComponents.cs"] = "1",
		["Assets/Deer/Scripts/Hotfix/HotfixBusiness/UI/BindComponents/UIRaceSelectionForm.BindComponents.cs"] = "1",
		["Assets/Deer/Scripts/Hotfix/HotfixBusiness/UI/BindComponents/UISettingAudioForm.BindComponents.cs"] = "1",
		["Assets/Deer/Scripts/Hotfix/HotfixBusiness/UI/BindComponents/UISettingGameOptionsForm.BindComponents.cs"] = "1",
		["Assets/Deer/Scripts/Hotfix/HotfixBusiness/UI/BindComponents/UISettingsForm.BindComponents.cs"] = "1",
		["Assets/Standard Assets/ThirdParty/DeerExample"] = "1",
		["Assets/Deer/Scripts/Hotfix/HotfixBusiness/Entity/NPC"] = "1",
		["Assets/Deer/Scripts/Hotfix/HotfixBusiness/Entity/Character/SphereCharacterPlayer.cs"] = "1",
		["Assets/Deer/Scripts/Hotfix/HotfixBusiness/Entity/CharacterData/SphereCharacterPlayerData.cs"] = "1",
		["Assets/Deer/Scripts/Hotfix/HotfixBusiness/Entity/StateFSM"] = "1",
		["Assets/Deer/Scripts/Hotfix/HotfixBusiness/Entity/UI"] = "1",
		["Assets/Deer/Scripts/Hotfix/HotfixBusiness/GameLogic"] = "1",
		
		["Assets/Deer/Scripts/Hotfix/HotfixBusiness/Procedure/ProcedureGamePlay.cs"] = "1",

	};
	// 将"Assets/MyFolder"移动到“项目根路径/MyFolder”
	private static string m_DestFolderPath = Application.dataPath + "/../DeerExample/";
	[MenuItem("DeerTools/DeerExample/AddExample")]
	public static void AddDeerExample()
	{
		foreach (var dicExample in m_DicExamplePaths)
		{
			string srcFolderPath = m_DestFolderPath + dicExample.Key;
			string strAsset = "Assets";
			string destFolderPath = Application.dataPath + dicExample.Key.Remove(dicExample.Key.IndexOf(strAsset),strAsset.Length);
			if (Directory.Exists(srcFolderPath))
			{
				FolderUtils.CopyFolder(srcFolderPath,destFolderPath);
			}

			if (File.Exists(srcFolderPath))
			{
				FileInfo destFileInfo = new FileInfo(destFolderPath);
				if (destFileInfo.Directory != null && !destFileInfo.Directory.Exists)
				{
					destFileInfo.Directory.Create();
				}
				File.Copy(srcFolderPath, destFolderPath, true);
			}
		}
		Enable();
		DeerSettingsUtils.DeerGlobalSettings.m_UseDeerExample = true;
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
		
	}
	[MenuItem("DeerTools/DeerExample/RemoveExample")]
	public static void RemoveDeerExample()
	{
		foreach (var dicExample in m_DicExamplePaths)
		{
			string destFolderPath = m_DestFolderPath +dicExample.Key;
			string strAsset = "Assets";
			string srcFolderPath = Application.dataPath+dicExample.Key.Remove(dicExample.Key.IndexOf(strAsset),strAsset.Length);
			if (Directory.Exists(srcFolderPath))
			{
				FolderUtils.CopyFolder(srcFolderPath,destFolderPath);
				FileUtil.DeleteFileOrDirectory(srcFolderPath);
				File.Delete(srcFolderPath+".meta");
			}

			if (File.Exists(srcFolderPath))
			{
				FileInfo destFileInfo = new FileInfo(destFolderPath);
				if (destFileInfo.Directory != null && !destFileInfo.Directory.Exists)
				{
					destFileInfo.Directory.Create();
				}
				File.Copy(srcFolderPath,destFolderPath, true);
				FileUtil.DeleteFileOrDirectory(srcFolderPath);
				File.Delete(srcFolderPath+".meta");
			}
		}
		Disable();
		DeerSettingsUtils.DeerGlobalSettings.m_UseDeerExample = false;
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}

	public static bool IsUseDeerExampleInProject()
	{
		return DeerSettingsUtils.DeerGlobalSettings.m_UseDeerExample;
	}

	private static void Enable()
	{
		AddScriptingDefineSymbol();
		DeerSettingsUtils.DeerGlobalSettings.m_UseDeerExample = true;
	}

	private static void Disable()
	{
		bool isFind = false;
		foreach (var buildTargetGroup in BuildTargetGroups)
		{
			if (ScriptingDefineSymbols.HasScriptingDefineSymbol(buildTargetGroup,EnableEXAMPLE))
			{
				isFind = true;
			}
		}
		if (isFind)
		{
			AddScriptingDefineSymbol(false);
		}
	}
	private static void AddScriptingDefineSymbol(bool isAdd = true)
	{
		if (isAdd)
		{
			ScriptingDefineSymbols.AddScriptingDefineSymbol(EnableEXAMPLE);
		}
		else
		{
			ScriptingDefineSymbols.RemoveScriptingDefineSymbol(EnableEXAMPLE);
		}
	}
}