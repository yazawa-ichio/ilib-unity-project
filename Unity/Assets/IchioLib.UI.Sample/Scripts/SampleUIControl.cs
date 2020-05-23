using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ILib.UI;
using ILib.MVVM;
using System.Threading.Tasks;

namespace ILib.Sample.UI
{

	public class SampleUIControl : UIControl<IViewModel>, IExecuteBack
	{
		[EventKey]
		public enum Event
		{
			Close,
			Push,
			Change,
			Enqueue,
		}

		IView m_View;
		public IViewModel Context { get; private set; }

		protected sealed override Task OnCreated(IViewModel prm)
		{
			Context = prm;
			m_View = GetComponent<IView>();
			m_View.Attach(prm);
			return OnCreatedImpl();
		}

		protected virtual Task OnCreatedImpl()
		{
			Context.Event.Subscribe(Event.Close, () =>
			{
				if (CanBack()) Close();
			});
			return Util.Successed;
		}

		protected virtual bool CanBack()
		{
			return true;
		}

		bool IExecuteBack.TryBack()
		{
			Debug.Log("TryBack");
			if (CanBack())
			{
				Close();
				return true;
			}
			return false;
		}

	}

}