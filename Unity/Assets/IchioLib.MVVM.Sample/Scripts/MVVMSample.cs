using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ILib.MVVM;

namespace ILib.Sample.MVVM
{

	public class MVVMSample : MonoBehaviour
	{
		[EventKey]
		public enum Event
		{
			Submit,
		}

		System.IDisposable m_Disposable;

		void Start()
		{
			int count = 0;
			GetComponent<View>().Attach<MVVMSampleVM>(vm =>
			{
				vm.Text.Value = "Click !";
				vm.Button = new DelegateCommand(() =>
				{
					count++;
					vm.Text.Value = "Clicked";
					var item = new SampleMVVMTextVM { Text = "test" + count };
					item.OnButton += (index) =>
					{
						Debug.Log(index);
					};
					vm.Collection.Add(item);
					vm.ToggleMessage.Value = count % 2 == 0;
					m_Disposable.Dispose();
				});
				vm.Slider = new DelegateCommand<float>(v => vm.Position.Value = new Vector3(0, v * 10f, 0));
				vm.Submit = new DelegateCommand<string>(Log);
				//vm.Messenger.Register<Event, string>(this, Event.Submit, OnSubmit);
				m_Disposable = vm.Messenger.WeakRegisterHandle(this);
				vm.Messenger.Register<string>(this, Event.Submit, (x) => OnSubmit(x));
				(vm as IViewModel).Event.Subscribe<Event, string>(Event.Submit, (x) => OnSubmit(x));
			});
		}

		void Log(string v)
		{
			Debug.Log(v);
		}

		[MessageHandle(Event.Submit)]
		void OnSubmit(string val)
		{
			Debug.Log(val);
		}

	}

}