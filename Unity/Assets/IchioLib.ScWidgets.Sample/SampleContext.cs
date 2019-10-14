using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ILib.Debugs;
using ILib.Debugs.AutoRegisters;

namespace ILib.Sample.Debugs
{
	[AutoRegisterTarget]
	public class SampleContext : MonoBehaviour
	{
		ScDebugMenu m_DebugMenu;

		public string SampleInput;

		[DebugToggle("Toggle")]
		public bool Toggle;

		[DebugInput("InputTest")]
		public string InputTest;

		[DebugSlider("DebugSlider")]
		public float SliderTest = 0.5f;

		[DebugToggle("Toggle2")]
		public bool Toggle2;

		[DebugInput("InputTest2")]
		public string InputTest2;

		[DebugSlider("DebugSlider2",Path = "PathTest")]
		public float SliderTest2 = 0.5f;

		[DebugToggle("Toggle3", Path = "PathTest")]
		public bool Toggle3;

		[DebugInput("InputTest3", Path = "PathTest/PathTest")]
		public string InputTest3;

		[DebugSlider("DebugSlider3", Path = "PathTest/PathTest")]
		public float SliderTest3 = 0.5f;

		[DebugLabel("GetTime")]
		public string GetTime() => Time.time.ToString();

		[DebugLabel("GetDeltaTime")]
		public string GetDeltaTime() => Time.deltaTime.ToString();

		[DebugToggle("Toggle4")]
		public bool Toggle4;

		[DebugInput("InputTest4")]
		public string InputTest4;

		[DebugSlider("DebugSlider4")]
		public float SliderTest4 = 0.5f;

		[DebugToggle("Toggle5")]
		public bool Toggle5;

		[DebugInput("InputTest5")]
		public string InputTest5;

		[DebugSlider("DebugSlider5")]
		public float SliderTest5 = 0.5f;


		public int SelectIndex;

		private void Awake()
		{
			m_DebugMenu = GetComponent<ScDebugMenu>();
			m_DebugMenu.Contexts.Bind(this);
		}

		private void Update()
		{
#if UNITY_EDITOR
			if (Input.GetMouseButtonUp(1))
			{
				m_DebugMenu.Open();
			}
#else
			if (Input.touchCount == 4)
			{
				m_DebugMenu.Open();
			}
#endif
		}

		public void OnButton()
		{
			Debug.Log("OnButton");
		}

		[DebugButton("TestButton")]
		void Test()
		{
			Debug.Log("Test");
		}

		[DebugButton("TestButton2", Path ="PathTest/Button")]
		void Test2()
		{
			Debug.Log("Test2");
		}

	}

	public class SampleInputContent : InputFieldContent<SampleContext>
	{
		protected override string Label => "サンプル入力";

		protected override string Value
		{
			get => Context.SampleInput;
			set => Context.SampleInput = value;
		}

		protected override string Category => "Sample";
	}

	public class SampleButtonContent : ButtonContent<SampleContext>
	{
		protected override string Category => "カテゴリテスト";
		protected override string Label => "サンプルボタン";
		protected override string Path => "TestPage/NestPage";

		protected override void OnClick()
		{
			Context.OnButton();
		}
	}

	public class SampleButtonContent2 : ButtonContent<SampleContext>
	{
		protected override string Category => "カテゴリテスト";
		protected override string Label => "サンプルボタン2";
		protected override string Path => "TestPage/NestPage";

		protected override void OnClick()
		{
			Context.OnButton();
		}
	}

	public class SampleButtonContent3 : ButtonContent<SampleContext>
	{
		protected override string Category => "カテゴリテスト2";
		protected override string Label => "サンプルボタン3";
		protected override string Path => "TestPage";

		protected override void OnClick()
		{
			Context.OnButton();
		}
	}

	public class SampleButtonContent4 : ButtonContent<SampleContext>
	{
		protected override string Category => "カテゴリテスト";
		protected override string Label => "サンプルボタン4";
		protected override string Path => "TestPage/NestPage";

		protected override void OnClick()
		{
			Context.OnButton();
		}
	}

	public class SampleButtonContent5 : ButtonContent<SampleContext>
	{
		protected override string Category => "カテゴリテスト2";
		protected override string Label => "サンプルボタン5";
		protected override string Path => "@JumpPage";

		protected override void OnClick()
		{
			Context.OnButton();
		}
	}


	public class SamplePageJumpContent : PageJumpContent<SampleContext>
	{
		protected override string Category => "カテゴリテスト4";
		protected override string Label => "JumpPage";
		protected override string Path => "TestPage/NestPage";
	}

	public class SampleCheckBoxContent : CheckBoxContent<SampleContext>
	{
		protected override string Label => "チェックボックス";

		protected override bool Value { get => Context.Toggle; set => Context.Toggle = value; }
	}

	public class SampleRadioButtonContent : RadioButtonContent<SampleContext>
	{
		protected override string[] Items => new string[] {
			"RadioButton1",
			"RadioButton2",
			"RadioButton3",
			"RadioButton4",
			"RadioButton5"
		};

		protected override int Select => Context.SelectIndex;

		protected override void OnChanged(int index, string name)
		{
			Context.SelectIndex = index;
		}
	}


}
