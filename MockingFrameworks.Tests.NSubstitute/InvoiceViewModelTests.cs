using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Shared;
using Shouldly;
using MockingFrameworks.Pages.Invoice;
using MockingModels;

namespace MockingFrameworks.Tests.NSubstitute
{
	[TestClass]
	public class InvoiceViewModelTests
	{
		protected InvoiceViewModelTestsHelpers Helpers { get; set; }
		protected InvoiceViewModel ViewModel { get; set; }
		protected IInvoiceRepository InvoiceRepository { get; set; }
		protected IUserRepository UserRepository { get; set; }
		protected IProductRepository ProductRepository { get; set; }
		protected ILog Logger { get; set; }

		[TestInitialize]
		public void Initialize()
		{
			Helpers = new InvoiceViewModelTestsHelpers();

			InvoiceRepository = Substitute.For<IInvoiceRepository>();
			UserRepository = Substitute.For<IUserRepository>();
			ProductRepository = Substitute.For<IProductRepository>();
			Logger = Substitute.For<ILog>();

			ViewModel = new InvoiceViewModel(InvoiceRepository, UserRepository, ProductRepository, Logger);
		}

		[TestMethod]
		public void InvoiceViewModel_SaveInvoice_SavesInvoiceWithCorrectData()
		{
			// Setup
			var products = Helpers.GenerateProducts(50);
			var users = Helpers.GenerateUsers(10, products);
			var selectedProducts = products.Where(p => p.ProductId % 3 == 0);
			int invoiceId = 42;
			var selectedUser = users.ElementAt(2);

			ProductRepository.All().Returns(products);
			UserRepository.All().Returns(users);
			InvoiceRepository.Save(Arg.Any<Invoice>()).WhenForAnyArgs(i => { i.InvoiceId = invoiceId; });
			Logger.MinimumLevel.Returns(LogLevel.Warning);

			foreach (var product in selectedProducts)
				ViewModel.CreatingInvoice.Products.Add(product);

			ViewModel.SelectedUser = selectedUser;
			ViewModel.CreatingInvoice.Products.ShouldBeOneOf(selectedProducts.ToArray());

			// Invoke
			ViewModel.SaveInvoice();

			// Assert
			InvoiceRepository.Received(1).Save(Arg.Is<Invoice>(i => i.User == selectedUser));

			// Here we have to set a throw-away variable because the compiler won't like it otherwise
			var _ = Logger.Received(1).MinimumLevel;
			Logger.DidNotReceive().Debug(Arg.Any<string>());
			Logger.DidNotReceive().Information(Arg.Any<string>());
			Logger.DidNotReceive().Warning(Arg.Any<string>());
			Logger.DidNotReceive().Error(Arg.Any<string>());

			ViewModel.Errors.ShouldBeNull();
			ViewModel.CreatingInvoice.User.ShouldBeNull();
			ViewModel.CreatingInvoice.Products.ShouldBeNull();
		}

		[TestMethod]
		public void InvoiceViewModel_DoesntSaveInvoiceWithIncorrectData()
		{
			// Setup
			var products = Helpers.GenerateProducts(50);
			var users = Helpers.GenerateUsers(10, products);
			var selectedProducts = products.Where(p => p.ProductId % 3 == 0);
			var selectedUser = users.ElementAt(2);

			ProductRepository.All().Returns(products);
			UserRepository.All().Returns(users);
			Logger.MinimumLevel.Returns(LogLevel.Warning);

			ViewModel.SelectedUser = selectedUser;

			// Invoke
			ViewModel.SaveInvoice();

			// Assert
			ViewModel.Errors.ShouldNotBeEmpty();
			InvoiceRepository.DidNotReceive().Save(Arg.Any<Invoice>());
			Logger.Received(1).Warning(Arg.Any<string>());
			Logger.DidNotReceive().Debug(Arg.Any<string>());
			Logger.DidNotReceive().Information(Arg.Any<string>());
			Logger.DidNotReceive().Error(Arg.Any<string>());

			foreach (var product in selectedProducts)
				ViewModel.CreatingInvoice.Products.Add(product);

			ViewModel.CreatingInvoice.Products.ShouldBeOneOf(selectedProducts.ToArray());

			// Invoke
			ViewModel.SaveInvoice();

			// Assert
			InvoiceRepository.Received(1).Save(Arg.Is<Invoice>(i => i.User == selectedUser));
			Logger.Received(2).Warning(Arg.Any<string>());
			Logger.DidNotReceive().Debug(Arg.Any<string>());
			Logger.DidNotReceive().Information(Arg.Any<string>());
			Logger.DidNotReceive().Error(Arg.Any<string>());

			ViewModel.CreatingInvoice.User.ShouldBeNull();
			ViewModel.CreatingInvoice.Products.ShouldBeNull();
		}

		[TestMethod]
		public void InvoiceViewModel_SaveInvoice_HandlesExceptionWhenSelectedUserNull()
		{

			var products = Helpers.GenerateProducts(50);
			var users = Helpers.GenerateUsers(10, products);
			var selectedProducts = products.Where(p => p.ProductId % 3 == 0);

			ProductRepository.All().Returns(products);
			UserRepository.All().Returns(users);
			Logger.MinimumLevel.Returns(LogLevel.Warning);
			InvoiceRepository.When(repo => repo.Save(Arg.Any<Invoice>())).Do(repo => throw new ArgumentException("User", "User cannot be null"));


			ViewModel.SaveInvoice();

			InvoiceRepository.Received(1).Save(Arg.Any<Invoice>());

			ViewModel.Errors.ShouldNotBeNull();
			ViewModel.Errors.ShouldNotBeEmpty();
		}

		[TestMethod]
		public void InvoiceViewModel_UserSelectedLoadsCommonProductsForUserThatHasInvoices()
		{
			var products = Helpers.GenerateProducts(15);
			var user = Helpers.GenerateUsers(1, products).First();

			InvoiceRepository.HasInvoices(user.UserId, out var invoices).Returns(x =>
			{
				x[1] = new List<Invoice>();
				return false;
			});

			ViewModel.SelectedUser = user;

			ViewModel.UserSelected();

			ViewModel.CanShowUserInvoices.ShouldBeTrue();
			ViewModel.PreviouslyUsedProducts.ShouldNotBeEmpty();
			ViewModel.PreviouslyUsedProducts.ShouldAllBe(p => products.Contains(p));
		}

		[TestMethod]
		public void InvoiceViewModel_UserSelectedHidesProductsForUserWithoutInvoices()
		{
			var user = Helpers.GenerateUsers(1).First();

			ViewModel.SelectedUser = user;

			ViewModel.UserSelected();

			ViewModel.CanShowUserInvoices.ShouldBeFalse();
		}
	}
}
