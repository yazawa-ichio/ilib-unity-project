using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ILib.MVVM;

namespace ILib.Sample.MVVM
{

	public class MVVMSample : MonoBehaviour
	{
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
					vm.Collection.Add(new SampleMVVMTextVM { Text = "test"+count });
				});
				vm.Slider = new DelegateCommand<float>(v => vm.Position.Value = new Vector3(0, v * 10f, 0));
			});

		}
	}

}
