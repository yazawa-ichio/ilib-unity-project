﻿using UnityEngine;

namespace ILib.Audio.SoundManagement
{
	/// <summary>
	/// サウンドマネージャーの起動を行うスクリプトです。
	/// </summary>
	public class SoundManagerInstaller : MonoBehaviour
	{
		[SerializeField]
		ConfigAsset m_Config = null;

		[SerializeField]
		bool m_DontDestroy = false;

		private void Awake()
		{
			SoundManager.Release();
			Init();
		}

		public void Init()
		{
			var loader = GetComponent<ISoundLoader>();
			SoundManager.Initialize(m_Config.GetConfig(loader));
			if (m_DontDestroy)
			{
				GameObject.DontDestroyOnLoad(gameObject);
			}
		}

		private void OnDestroy()
		{
			SoundManager.Release();
		}

	}

}