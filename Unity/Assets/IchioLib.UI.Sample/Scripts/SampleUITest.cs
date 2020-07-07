using ILib.MVVM;
using System;
using UnityEngine;

namespace ILib.Sample.UI
{

	public class SampleUITest : MonoBehaviour
	{
		[SerializeField]
		SampleUIStack m_UIStack = default;

		[SerializeField]
		SampleUIQueue m_UIQueue = default;

		private async void Start()
		{
			ILib.UI.UIControlLog.Init();
			await m_UIStack.Push("SampleUI", (GeneralViewModel vm) =>
			{
				vm.Command(SampleUIControl.Event.Push, () =>
				{
					Push();
				});
				vm.Command(SampleUIControl.Event.Change, () =>
				{
					m_UIStack.Switch("SampleUI2", new GeneralViewModel());
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
			await m_UIStack.Push("SampleUI", (GeneralViewModel vm) =>
			{
				vm.Command(SampleUIControl.Event.Push, () =>
				{
					Push();
				});
				vm.Command(SampleUIControl.Event.Change, () =>
				{
					m_UIStack.Switch("SampleUI2", new GeneralViewModel());
				});
				vm.Command(SampleUIControl.Event.Enqueue, async () =>
				{
					var test1 = m_UIQueue.Enqueue("SampleUI", new GeneralViewModel());
					var test2VM = new GeneralViewModel();
					test2VM.Set("QueryResult", "QueryResultTest");
					var test2 = m_UIQueue.Query<string>("SampleUI2", test2VM);
					var test3 = m_UIQueue.Enqueue("SampleUI", new GeneralViewModel());
					try
					{
						await test1;
					}
					catch (Exception ex)
					{
						Debug.LogError(ex);
						m_UIQueue.RepairError();
					}
					try
					{
						var test = await test2;
						Debug.LogError(test);
					}
					catch (Exception ex)
					{
						Debug.LogError(ex);
						m_UIQueue.RepairError();
					}
					try
					{
						await test3;
					}
					catch (Exception ex)
					{
						Debug.LogError(ex);
						m_UIQueue.RepairError();
					}
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