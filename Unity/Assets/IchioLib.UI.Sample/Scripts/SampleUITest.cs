using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ILib.MVVM;

namespace ILib.Sample.UI
{

	public class SampleUITest : MonoBehaviour
	{
		[SerializeField]
		SampleUIStack m_UIStack = default;

		[SerializeField]
		SampleUIQueue m_UIQueue = default;

		private void Start()
		{
			m_UIStack.Push("SampleUI", (ViewModel vm) =>
			{
				vm.SubscribeViewEvent(SampleUIControl.Event.Push, () =>
				{
					Push();
				});
				vm.SubscribeViewEvent(SampleUIControl.Event.Change, () =>
				{
					m_UIStack.Switch("SampleUI2", new ViewModel());
				});
			});
			/*
			var queue = m_UIStack.Push("SampleUI", new ViewModel());
			m_UIStack.Push("SampleUI", new ViewModel());
			m_UIStack.Switch("SampleUI", new ViewModel());
			m_UIStack.Switch("SampleUI", new ViewModel());
			m_UIStack.Push("SampleUI", new ViewModel());
			m_UIStack.Push("SampleUI", new ViewModel());
			*/
			
			/*
			var queue = m_UIQueue.Enqueue("SampleUI2", new ViewModel());
			m_UIQueue.Enqueue("SampleUI2", new ViewModel());
			m_UIQueue.Enqueue("SampleUI2", new ViewModel());
			m_UIQueue.Enqueue("SampleUI2", new ViewModel());
			m_UIQueue.Enqueue("SampleUI2", new ViewModel());
			m_UIQueue.Enqueue("SampleUI", new ViewModel());
			*/
		}

		async void Push()
		{
			await m_UIStack.Push("SampleUI", (ViewModel vm) =>
			{
				vm.SubscribeViewEvent(SampleUIControl.Event.Push, () => {
					Push();
				});
				vm.SubscribeViewEvent(SampleUIControl.Event.Change, () => {
					m_UIStack.Switch("SampleUI2", new ViewModel());
				});
				vm.SubscribeViewEvent(SampleUIControl.Event.Enqueue, () => {
					m_UIQueue.Enqueue("SampleUI", new ViewModel());
					m_UIQueue.Enqueue("SampleUI2", new ViewModel());
					m_UIQueue.Enqueue("SampleUI", new ViewModel());
				});
			});
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				if (m_UIQueue.ExecuteBack()) return;
				m_UIStack.ExecuteBack();
			}
		}

	}

}
