using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ILib.MVVM;
using ILib.UI;

namespace ILib.Sample.UI
{
	public class SampleUIStack : UIStack<IViewModel, IControl<IViewModel>>
	{
		CanvasGroup m_CanvasGroup;

		private void Awake()
		{
			m_CanvasGroup = GetComponent<CanvasGroup>();
		}

		protected override void OnStartProcess()
		{
			m_CanvasGroup.blocksRaycasts = false;
		}

		protected override void OnEndProcess()
		{
			m_CanvasGroup.blocksRaycasts = true;
		}

		public IStackEntry Push<T>(string path, System.Action<T> action) where T : IViewModel, new()
		{
			T vm = new T();
			action?.Invoke(vm);
			return Push(path, vm);
		}

		public IStackEntry Switch<T>(string path, System.Action<T> action) where T : IViewModel, new()
		{
			T vm = new T();
			action?.Invoke(vm);
			return Switch(path, vm);
		}

	}
}