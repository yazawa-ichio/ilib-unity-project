using ILib.MVVM;
using ILib.UI;
using System.Threading.Tasks;
using UnityEngine;

namespace ILib.Sample.UI
{

	public class SampleUIControl : UIControl<IViewModel>, IExecuteBack, IQueueQueryHandler
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

		public TResult GetResult<TResult>()
		{
			return Context.Get<TResult>("QueryResult");
		}

	}

}