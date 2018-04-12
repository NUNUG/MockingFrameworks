using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using MockingFrameworks.Shell;
using MockingFrameworks.Models;

namespace MockingFrameworks.Tests.NSubstitute
{
	[TestClass]
	public class ShellViewModelTests
	{
		protected ShellViewModel ViewModel { get; set; }

		[TestMethod]
		public void ShellViewModel_SetsFirstItem()
		{
			var screens = new List<IconScreen>();
			for(int i = 0; i < 5; ++i)
			{
				var screen = Substitute.For<IconScreen>();
				screen.Icon = $"Icon {i}";

				screens.Add(screen);
			}

			ViewModel = new ShellViewModel(screens);
			ViewModel.SelectedItem.ShouldBeSameAs(screens.First());
		}
	}
}
