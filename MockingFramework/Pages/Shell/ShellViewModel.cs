using Caliburn.Micro;
using MockingFramework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MockingFramework.Shell
{
	public class ShellViewModel : Conductor<IconScreen>.Collection.OneActive
	{
		private IconScreen _SelectedItem;

		public IconScreen SelectedItem
		{
			get { return _SelectedItem; }
			set { _SelectedItem = value; NotifyOfPropertyChange(nameof(SelectedItem)); }
		}


		public ShellViewModel(IEnumerable<IconScreen> screens)
		{
			Items.AddRange(screens);

			if (Items.Count > 0)
			{
				SelectedItem = Items.First();
				ActivateItem(SelectedItem);
			}
		}

		public Task NavigationChanged(Windows.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
		{
			var screen = args.SelectedItem as IconScreen;
			var selectedScreen = Items.FirstOrDefault(i => i.DisplayName == screen?.DisplayName);

			ActivateItem(selectedScreen);
			return Task.CompletedTask;
		}
	}
}
