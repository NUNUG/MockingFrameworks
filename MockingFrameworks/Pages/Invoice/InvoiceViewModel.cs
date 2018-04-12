using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MockingFrameworks.Models;
using MockingModels;

namespace MockingFrameworks.Pages.Invoice
{
	public class InvoiceViewModel : IconScreen
	{
		public IEnumerable<MockingModels.Invoice> UserInvoices
		{
			get { return SelectedUser?.Invoices; }
		}

		private bool _CanShowUserInvoices;
		public bool CanShowUserInvoices
		{
			get { return _CanShowUserInvoices; }
			set
			{
				_CanShowUserInvoices = value;
				NotifyOfPropertyChange(nameof(CanShowUserInvoices));
				NotifyOfPropertyChange(nameof(UserInvoices));
			}
		}

		private IEnumerable<Product> _PreviouslyUsedProducts;
		public IEnumerable<Product> PreviouslyUsedProducts
		{
			get { return _PreviouslyUsedProducts; }
			set { _PreviouslyUsedProducts = value; NotifyOfPropertyChange(nameof(PreviouslyUsedProducts)); }
		}


		protected ObservableCollection<User> _Users;
		public ObservableCollection<User> Users
		{
			get => _Users;
			set { _Users = value; NotifyOfPropertyChange(nameof(Users)); }
		}

		protected User _User;
		public User SelectedUser
		{
			get => _User;
			set
			{
				_User = value;
				NotifyOfPropertyChange(nameof(_User));
				NotifyOfPropertyChange(nameof(PreviouslyUsedProducts));
			}
		}

		protected MockingModels.Invoice _CreatingInvoice;
		public MockingModels.Invoice CreatingInvoice
		{
			get => _CreatingInvoice;
			set { _CreatingInvoice = value; NotifyOfPropertyChange(nameof(CreatingInvoice)); }
		}

		public IEnumerable<Product> Products { get; protected set; }

		protected IEnumerable<string> _Errors;
		public IEnumerable<string> Errors
		{
			get => _Errors;
			set { _Errors = value; NotifyOfPropertyChange(nameof(Errors)); }
		}

		protected IInvoiceRepository InvoiceRepository { get; }
		protected IUserRepository UserRepository { get; }
		protected IProductRepository ProductRepository { get; }
		protected ILog Logger { get; }

		public InvoiceViewModel(IInvoiceRepository invoiceRepository, IUserRepository userRepository, IProductRepository productRepository, ILog logger)
		{
			InvoiceRepository = invoiceRepository;
			UserRepository = userRepository;
			ProductRepository = productRepository;
			Logger = logger;

			DisplayName = "Invoices";
			Icon = "Invoice";
		}

		protected override void OnActivate()
		{
			base.OnActivate();

			ClearInvoice();
			Users = new ObservableCollection<User>(UserRepository.All());
			Products = new List<Product>(ProductRepository.All());
		}

		public void SaveInvoice()
		{
			var errors = new List<string>();

			if (CreatingInvoice.Products == null || CreatingInvoice.Products.Count == 0)
				errors.Add("You must include at least one product in the invoice");

			CreatingInvoice.User = SelectedUser;

			if(errors.Count > 0)
			{
				Errors = errors;

				if (Logger.MinimumLevel > LogLevel.Information)
					Logger.Warning($"Could not save invoice. {string.Join("\n\t", errors.ToArray())}");

				return;
			}

			try
			{
				InvoiceRepository.Save(CreatingInvoice);
				ClearInvoice();
			}
			catch (Exception e)
			{
				errors.Add(e.Message);
				Errors = errors;
			}
		}

		public void ClearInvoice()
		{
			CreatingInvoice = new MockingModels.Invoice()
			{
				Products = new List<Product>()
			};
		}

		public void UserSelected()
		{
			if(InvoiceRepository.HasInvoices(SelectedUser.UserId, out var invoices))
			{
				CanShowUserInvoices = true;
				var containProducts = invoices.Where(i => i.Products != null && i.Products.Count > 0);
				PreviouslyUsedProducts = containProducts.SelectMany(i => i.Products).Distinct();
			}
			else
			{
				CanShowUserInvoices = false;
			}
		}
	}
}
