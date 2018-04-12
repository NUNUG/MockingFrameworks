using MockingFramework.Models;
using MockingModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MockingFramework.Pages.Products
{
	public class ProductsViewModel : IconScreen
	{
		private ObservableCollection<Product> _Products;
		public ObservableCollection<Product> Products
		{
			get { return _Products; }
			set { _Products = value; NotifyOfPropertyChange(nameof(Products)); }
		}

		public ProductsViewModel()
		{
			DisplayName = "Products";
			Icon = "Product";
		}
	}
}
