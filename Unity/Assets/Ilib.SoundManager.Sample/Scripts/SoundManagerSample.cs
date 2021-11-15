using ILib.Debugs;
using ILib.Debugs.AutoRegisters;
using UnityEngine;

namespace ILib.Audio.Sample
{
	[AutoRegisterTarget]
	public class SoundManagerSample : MonoBehaviour
	{
		ScDebugMenu m_Menu;

		private void Awake()
		{
			m_Menu = GetComponent<ScDebugMenu>();
			m_Menu.Contexts.Bind(this);
			m_Menu.Open();
		}

		void Update()
		{
			if (Input.GetMouseButtonDown(1))
			{
				m_Menu.Open();
			}
		}

		[DebugButton("PlaySe")]
		void PlaySe()
		{
			SoundManager.Se.PlayHandle("test1");
		}

		[DebugButton("PlaySePitch")]
		void PlaySePitch()
		{
			SoundManager.Se.PlayHandle("test1").Pitch = 2f;
		}


		[DebugButton("PlaySe FadeIn")]
		void PlaySeFadeIn()
		{
			SoundManager.Se.PlayHandle("test2").FadeIn(2f);
		}


		[DebugButton("PlaySe FadeOut")]
		void PlaySeFadeOut()
		{
			SoundManager.Se.PlayHandle("test2").FadeOut(2f);
		}

		[DebugButton("Push", Path = "BGM")]
		void BgmPush()
		{
			SoundManager.Bgm.Push("test" + (1 + ((int)Time.time) % 2));
		}

		[DebugButton("Pop", Path = "BGM")]
		void BgmPop()
		{
			SoundManager.Bgm.Pop();
		}


		[DebugButton("Pause", Path = "BGM")]
		void BgmPause()
		{
			SoundManager.Bgm.Pause();
		}

		[DebugButton("Resume", Path = "BGM")]
		void BgmResume()
		{
			SoundManager.Bgm.Resume();
		}

		[DebugButton("PlayVoice")]
		void PlayVoice()
		{
			SoundManager.Voice.Play("test1");
		}

		[DebugButton("SetVolume")]
		void SetVolume()
		{
			SoundManager.BgmVolume = 0.1f;
		}


		float m_BgmVolume = 1f;
		[DebugSlider("BgmVolume")]
		public float BgmVolume
		{
			get => m_BgmVolume;
			set => m_BgmVolume = SoundManager.BgmVolume = value;
		}
	}

}