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



	}

}
