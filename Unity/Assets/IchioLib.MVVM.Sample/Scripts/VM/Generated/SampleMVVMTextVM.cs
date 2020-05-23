using System;
using System.Collections;
using System.Collections.Generic;
using ILib.MVVM;

namespace ILib.Sample.MVVM
{

	public partial class SampleMVVMTextVM : ViewModelBase
	{

		/// BindingPath : Text
		/// Target: ILib.MVVM.TextBind
		public string Text
		{
			get { return GetImpl<string>("Text"); }
			set { SetImpl<string>("Text", value); }
		}

		/// BindingPath : Button
		/// Target: ILib.MVVM.ButtonBind
		public bool ButtonValue
		{
			get { return GetImpl<bool>("Button"); }
			set { SetImpl<bool>("Button", value); }
		}

		/// BindingPath : Button
		/// Sender: ILib.MVVM.ButtonBind
		public event Action<int> OnButton
		{
			add
			{
				m_Event.Subscribe<int>("Button", value);
			}
			remove
			{
				m_Event.Unsubscribe<int>("Button", value);
			}
		}



	}

}