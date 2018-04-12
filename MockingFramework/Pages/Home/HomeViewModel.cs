﻿using Caliburn.Micro;
using MockingFramework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MockingFramework.Pages.Home
{
	public class HomeViewModel : IconScreen
	{
		public HomeViewModel()
		{
			DisplayName = "Home";
			Icon = "Play";
		}
	}
}
