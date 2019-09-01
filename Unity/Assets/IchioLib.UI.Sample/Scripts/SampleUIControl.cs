using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ILib.UI;
using ILib.MVVM;
using ILib.Routines;
using ILib;

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

		protected sealed override ITriggerAction OnCreated(IViewModel prm)
		{
			Context = prm;
			m_View = GetComponent<IView>();
			m_View.Attach(prm);
			return OnCreatedImpl();
		}

		protected virtual ITriggerAction OnCreatedImpl()
		{
			Context.SubscribeViewEvent(Event.Close, () => {
				if (CanBack()) Close();
			});
			return Trigger.Successed;
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
		/*
		protected override ITriggerAction OnFront(bool open)
		{
			return (Controller as MonoBehaviour).Routine(Move(new Vector3(0, open ? -500f : 500f, 0), Vector3.zero, 0.5f)).Action;
		}

		protected override ITriggerAction OnBehind()
		{
			return (Controller as MonoBehaviour).Routine(Move(Vector3.zero, new Vector3(0, 500f, 0), 0.5f)).Action;
		}

		protected override ITriggerAction OnClose()
		{
			return (Controller as MonoBehaviour).Routine(Move(Vector3.zero, new Vector3(0, -500f, 0), 0.5f)).Action;
		}

		IEnumerator Move(Vector3 from, Vector3 to, float time)
		{
			var pos = transform.localPosition;
			var cur = 0f;
			while (cur < time)
			{
				cur += Time.deltaTime;
				transform.localPosition = Vector3.Lerp(from, to, cur / time);
				yield return null;
			}
			transform.localPosition = to;
		}
		*/

	}

}
