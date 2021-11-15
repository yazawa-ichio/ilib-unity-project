using ILib.Audio.SoundManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace ILib.Audio
{
	/// <summary>
	/// ISoundPlayerとIMusicPlayerを利用した標準的なサウンドマネージャーのリファレンス実装です。
	/// </summary>
	public static class SoundManager
	{
		class SoundManagerInstance : MonoBehaviour
		{
			private void OnDestroy()
			{
				SoundManager.Release();
			}
		}

		/// <summary>
		/// BGMの再生用のコントローラーです
		/// </summary>
		public static BgmController Bgm { get; private set; }

		/// <summary>
		/// SEの再生用のコントローラーです
		/// </summary>
		public static SeController Se { get; private set; }

		/// <summary>
		/// Voiceの再生用のコントローラーです
		/// </summary>
		public static VoiceController Voice { get; private set; }

		public static float BgmVolume
		{
			set => SetVolume("CONFIG_BGM", value);
		}

		public static float SeVolume
		{
			set => SetVolume("CONFIG_SE", value);
		}

		public static float VoiceVolume
		{
			set => SetVolume("CONFIG_VOICE", value);
		}

		public static bool EnableDucking
		{
			set
			{
				var val = value ? SoundUtil.DecibelToVolume(-8) : 0;
				SetVolume("DUCK_LEVEL1", val);
				SetVolume("DUCK_LEVEL2", val);
				SetVolume("DUCK_LEVEL3", val);
			}
		}

		public static AudioMixer Mixer { get; private set; }

		static SoundManagerInstance s_Instance;
		static Coroutine s_TweenCoroutine;
		static List<MixerParamTweener> s_ParamTweener;

		public static bool IsValid() => s_Instance != null;

		public static void Initialize(Config config)
		{
			if (IsValid() && Application.isPlaying && s_Instance.hideFlags != HideFlags.None)
			{
				Release();
			}

			if (IsValid()) return;

			s_Instance = SoundControl.CreateRoot(nameof(SoundManager)).AddComponent<SoundManagerInstance>();

			Mixer = config.Mixer;

			Bgm = new BgmController(config);
			Se = new SeController(config);
			Voice = new VoiceController(config);

			s_ParamTweener = new List<MixerParamTweener>();

		}


		static MixerParamTweener GetTweener(string param, System.Func<float, float> inverseConv, System.Func<float, float> conv)
		{
			foreach (var t in s_ParamTweener)
			{
				if (t.Param == param)
				{
					return t;
				}
			}
			float val;
			Mixer.GetFloat(param, out val);
			val = inverseConv(val);
			var tweener = new MixerParamTweener(Mixer, param, val, conv);
			s_ParamTweener.Add(tweener);
			return tweener;
		}

		static void SetVolume(string param, float value, float time = 0.2f)
		{
			if (!IsValid())
			{
				Debug.Assert(false, "未初期化です");
				return;
			}
			MixerParamTweener tweener = GetTweener(param, x => SoundUtil.DecibelToVolume(x), x => SoundUtil.VolumeToDecibel(x));
			tweener.Start(value, time);
			if (s_TweenCoroutine == null)
			{
				s_TweenCoroutine = s_Instance.StartCoroutine(TweenCoroutine());
			}
		}

		public static void SetParamValue(string param, float value, float time = 0.2f)
		{
			if (!IsValid())
			{
				Debug.Assert(false, "未初期化です");
				return;
			}
			MixerParamTweener tweener = GetTweener(param, x => x, x => x);
			tweener.Start(value, time);
			if (s_TweenCoroutine == null)
			{
				s_TweenCoroutine = s_Instance.StartCoroutine(TweenCoroutine());
			}
		}

		static IEnumerator TweenCoroutine()
		{
			while (s_ParamTweener.Count > 0)
			{
				s_ParamTweener.RemoveAll(x => !x.Update());
				yield return null;
			}
			s_TweenCoroutine = null;
			yield break;
		}


		public static void Release()
		{
			if (!IsValid()) return;
			if (s_Instance.hideFlags != HideFlags.None)
			{
				GameObject.DestroyImmediate(s_Instance.gameObject);
			}
			else
			{
				GameObject.Destroy(s_Instance.gameObject);
			}
			s_Instance = null;
			Mixer = null;
			if (s_ParamTweener != null)
			{
				foreach (var val in s_ParamTweener)
				{
					val.Dispose();
				}
			}
			s_ParamTweener = null;

			Bgm?.Release();
			Bgm = null;

			Se?.Release();
			Se = null;

			Voice?.Release();
			Voice = null;
		}

#if UNITY_EDITOR
		public static void ForceUpdateInEditor()
		{
			SoundControl.ForceUpdateInEditor();
		}
#endif
	}
}