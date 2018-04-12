using Caliburn.Micro;
using MockingData;
using MockingFrameworks.Models;
using MockingFrameworks.Pages.Home;
using MockingFrameworks.Pages.Invoice;
using MockingFrameworks.Pages.Products;
using MockingFrameworks.Pages.Users;
using MockingFrameworks.Shell;
using MockingModels;
using MockingUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MockingFrameworks
{
	internal class Bootstrapper: BootstrapperBase
	{
		protected SimpleContainer Container { get; }

		public Bootstrapper()
		{
			Container = new SimpleContainer();

			Initialize();
		}

		protected override void BuildUp(object instance) => Container.BuildUp(instance);
		protected override IEnumerable<object> GetAllInstances(Type service) => Container.GetAllInstances(service);
		protected override object GetInstance(Type service, string key) => Container.GetInstance(service, key);

		protected override void Configure()
		{
			Container.Singleton<ShellViewModel>();
			Container.PerRequest<IconScreen, HomeViewModel>();
			Container.PerRequest<IconScreen, UsersViewModel>();
			Container.PerRequest<IconScreen, InvoiceViewModel>();
			Container.PerRequest<IconScreen, ProductsViewModel>();
			Container.PerRequest<IUserRepository, UserRepository>();
			Container.PerRequest<IProductRepository, ProductRepository>();
			Container.PerRequest<IInvoiceRepository, InvoiceRepository>();
			Container.Singleton<MockingModels.ILog, FileLogger>();
		}

		protected override void OnStartup(object sender, StartupEventArgs e)
		{
			DisplayRootViewFor<ShellViewModel>();
		}
	}
}
