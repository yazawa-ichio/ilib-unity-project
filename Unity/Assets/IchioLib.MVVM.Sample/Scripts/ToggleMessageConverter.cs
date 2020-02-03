using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ILib.MVVM
{

	public class ToggleMessageConverter : Converter<bool, string>
	{
		[SerializeField]
		string m_On = null;
		[SerializeField]
		string m_Off = null;

		public override string Convert(bool input)
		{
			return input ? m_On : m_Off;
		}

	}

}