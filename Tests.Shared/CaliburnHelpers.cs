using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Tests.Shared
{
	public class CaliburnHelpers
	{
		public void ActivateViewModel<T>(T viewModel) where T : class
		{
			if (viewModel == null)
				return;

			var type = viewModel.GetType();
			var activateMethod = type.GetMethod("OnActivate", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			activateMethod.Invoke(viewModel, new object[0]);
		}
	}
}
