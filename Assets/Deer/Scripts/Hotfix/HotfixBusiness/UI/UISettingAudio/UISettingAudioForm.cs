// ================================================
//描 述:
//作 者:AlanDu
//创建时间:2022-11-11 15-15-10
//修改作者:AlanDu
//修改时间:2022-11-11 15-15-10
//版 本:0.1 
// ===============================================

using HotfixFramework.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HotfixBusiness.UI
{
	/// <summary>
	/// 设置-音频设置 界面
	/// </summary>
	public partial class UISettingAudioForm : UIFixBaseForm
	{
		protected override void OnInit(object userData) {
			 base.OnInit(userData);
			 GetBindComponents(gameObject);

			m_Slider_MusicAudio.onValueChanged.AddListener(OnMasterVolumeSliderChange);
			m_Slider_SFXAudio.onValueChanged.AddListener(OnSFXVolumeSliderChange);
			m_Slider_UIAudio.onValueChanged.AddListener(OnMusicVolumeSliderChange);

			/*--------------------Auto generate start button listener.Do not modify!--------------------*/
			m_Btn_Back.onClick.AddListener(Btn_BackEvent);
/*--------------------Auto generate end button listener.Do not modify!----------------------*/
		}

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);

			m_Slider_MusicAudio.value = GameEntry.Sound.GetVolume(Constant.SoundGroup.Music);
			m_Slider_SFXAudio.value = GameEntry.Sound.GetVolume(Constant.SoundGroup.Sound);
			m_Slider_UIAudio.value = GameEntry.Sound.GetVolume(Constant.SoundGroup.UISound);
		}


		private void OnMasterVolumeSliderChange(float value)
		{
			GameEntry.Sound.SetVolume(Constant.SoundGroup.Music, value);
		}

		private void OnSFXVolumeSliderChange(float value)
		{
			GameEntry.Sound.SetVolume(Constant.SoundGroup.Sound, value);
		}

		private void OnMusicVolumeSliderChange(float value)
		{
			GameEntry.Sound.SetVolume(Constant.SoundGroup.UISound, value);
		}

		private void Btn_BackEvent(){
			Close();
		}
/*--------------------Auto generate footer.Do not add anything below the footer!------------*/
	}
}
