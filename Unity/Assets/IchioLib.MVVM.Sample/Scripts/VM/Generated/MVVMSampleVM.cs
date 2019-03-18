using System;
using System.Collections;
using System.Collections.Generic;
using ILib.MVVM;

namespace ILib.Sample.MVVM
{

	public partial class MVVMSampleVM : ViewModelBase
	{

		/// BindingPath : Collection
		/// Target: Content (ILib.MVVM.CollectionBind)
		public ReactiveListProperty<ILib.MVVM.IViewModel> Collection
		{
			get
			{
				if(m_Collection == null) m_Collection = new ReactiveListProperty<ILib.MVVM.IViewModel>("Collection", this);
				return m_Collection;
			}
		}
		ReactiveListProperty<ILib.MVVM.IViewModel> m_Collection;

		/// BindingPath : Text
		/// Target: ILib.MVVM.TextBind
		public ReactiveProperty<string> Text
		{
			get
			{
				if(m_Text == null) m_Text = new ReactiveProperty<string>("Text", this);
				return m_Text;
			}
		}
		ReactiveProperty<string> m_Text;

		/// BindingPath : Button
		/// Target: ILib.MVVM.ButtonBind
		public ReactiveProperty<bool> ButtonValue
		{
			get
			{
				if(m_ButtonValue == null) m_ButtonValue = new ReactiveProperty<bool>("Button", this);
				return m_ButtonValue;
			}
		}
		ReactiveProperty<bool> m_ButtonValue;

		private BindingCommand m_Button;

		/// BindingPath : Button
		/// Sender: ILib.MVVM.ButtonBind
		public ICommand Button
		{
			get
			{
				return m_Button?.Get() ?? null;
			}
			set
			{
				if (m_Button == null) m_Button = new BindingCommand("Button", this);
				m_Button.Set(value);
			}
		}

		/// BindingPath : Slider
		/// Target: ILib.MVVM.SliderBind
		public ReactiveProperty<float> SliderValue
		{
			get
			{
				if(m_SliderValue == null) m_SliderValue = new ReactiveProperty<float>("Slider", this);
				return m_SliderValue;
			}
		}
		ReactiveProperty<float> m_SliderValue;

		private BindingCommand<float> m_Slider;

		/// BindingPath : Slider
		/// Sender: ILib.MVVM.SliderBind
		public ICommand<float> Slider
		{
			get
			{
				return m_Slider?.Get() ?? null;
			}
			set
			{
				if (m_Slider == null) m_Slider = new BindingCommand<float>("Slider", this);
				m_Slider.Set(value);
			}
		}

		/// BindingPath : Position
		/// Target: ILib.MVVM.PostionBind
		public ReactiveProperty<UnityEngine.Vector3> Position
		{
			get
			{
				if(m_Position == null) m_Position = new ReactiveProperty<UnityEngine.Vector3>("Position", this);
				return m_Position;
			}
		}
		ReactiveProperty<UnityEngine.Vector3> m_Position;



	}

}
