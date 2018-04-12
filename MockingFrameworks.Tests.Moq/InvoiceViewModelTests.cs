using Microsoft.VisualStudio.TestTools.UnitTesting;
using MockingFramework.Pages.Invoice;
using MockingModels;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Shared;
using Shouldly;

namespace MockingFrameworks.Tests.Moq
{
	[TestClass]
	public class InvoiceViewModelTests
	{
		protected InvoiceViewModelTestsHelpers Helpers { get; set; }
		protected InvoiceViewModel ViewModel { get; set; }
		protected Mock<IInvoiceRepository> InvoiceRepository { get; set; }
		protected Mock<IUserRepository> UserRepository { get; set; }
		protected Mock<IProductRepository> ProductRepository { get; set; }
		protected Mock<ILog> Logger { get; set; }

		[TestInitialize]
		public void Initialize()
		{
			Helpers = new InvoiceViewModelTestsHelpers();

			InvoiceRepository = new Mock<IInvoiceRepository>();
			UserRepository = new Mock<IUserRepository>();
			ProductRepository = new Mock<IProductRepository>();
			Logger = new Mock<ILog>();
		}

		[TestMethod]
		public void InvoiceViewModel_SaveInvoice_SavesInvoiceWithCorrectData()
		{
			var products = Helpers.GenerateProducts(50);
			var users = Helpers.GenerateUsers(10, products);
			var selectedProducts = products.Where(p => p.ProductId % 3 == 0);
			int invoiceId = 42;
			var selectedUser = users.ElementAt(2);

			ProductRepository.Setup(repo => repo.All()).Returns(products);
			UserRepository.Setup(repo => repo.All()).Returns(users);
			InvoiceRepository.Setup(repo => repo.Save(It.IsAny<Invoice>())).Callback((Invoice i) => { i.InvoiceId = invoiceId; });
			Logger.Setup(log => log.MinimumLevel).Returns(LogLevel.Warning);

			ViewModel = new InvoiceViewModel(InvoiceRepository.Object, UserRepository.Object, ProductRepository.Object, Logger.Object)
			{
				SelectedUser = selectedUser
			};

			foreach (var product in selectedProducts)
				ViewModel.CreatingInvoice.Products.Add(product);

			ViewModel.CreatingInvoice.Products.ShouldBeOneOf(selectedProducts.ToArray());

			ViewModel.SaveInvoice();


			InvoiceRepository.Verify(repo => repo.Save(It.Is<Invoice>(i => i.User == selectedUser)), Times.Once());
			Logger.VerifyGet(log => log.MinimumLevel, Times.AtLeastOnce());
			Logger.Verify(log => log.Debug(It.IsAny<string>()), Times.Never());
			Logger.Verify(log => log.Information(It.IsAny<string>()), Times.Never());
			Logger.Verify(log => log.Warning(It.IsAny<string>()), Times.Never());
			Logger.Verify(log => log.Error(It.IsAny<string>()), Times.Never());

			ViewModel.Errors.ShouldBeNull();
			ViewModel.CreatingInvoice.User.ShouldBeNull();
			ViewModel.CreatingInvoice.Products.ShouldBeNull();
		}

		[TestMethod]
		public void InvoiceViewModel_SaveInvoice_DoesntSaveInvoiceWithIncorrectData()
		{
			var products = Helpers.GenerateProducts(50);
			var users = Helpers.GenerateUsers(10, products);
			var selectedProducts = products.Where(p => p.ProductId % 3 == 0);
			var selectedUser = users.ElementAt(2);

			ProductRepository.Setup(repo => repo.All()).Returns(products);
			UserRepository.Setup(repo => repo.All()).Returns(users);
			Logger.Setup(log => log.MinimumLevel).Returns(LogLevel.Warning);

			ViewModel = new InvoiceViewModel(InvoiceRepository.Object, UserRepository.Object, ProductRepository.Object, Logger.Object)
			{
				SelectedUser = selectedUser
			};

			ViewModel.SaveInvoice();

			ViewModel.Errors.ShouldNotBeEmpty();
			Logger.Verify(log => log.Warning(It.IsAny<string>()), Times.Once());
			Logger.Verify(log => log.Debug(It.IsAny<string>()), Times.Never());
			Logger.Verify(log => log.Information(It.IsAny<string>()), Times.Never());
			Logger.Verify(log => log.Error(It.IsAny<string>()), Times.Never());
			InvoiceRepository.Verify(repo => repo.Save(It.IsAny<Invoice>()), Times.Never());

			foreach (var product in selectedProducts)
				ViewModel.CreatingInvoice.Products.Add(product);

			ViewModel.CreatingInvoice.Products.ShouldBeOneOf(selectedProducts.ToArray());

			ViewModel.SaveInvoice();

			Logger.Verify(log => log.Warning(It.IsAny<string>()), Times.Exactly(2));
			Logger.Verify(log => log.Debug(It.IsAny<string>()), Times.Never());
			Logger.Verify(log => log.Information(It.IsAny<string>()), Times.Never());
			Logger.Verify(log => log.Error(It.IsAny<string>()), Times.Never());
			InvoiceRepository.Verify(repo => repo.Save(It.Is<Invoice>(i => i.User == selectedUser)), Times.Never());

			ViewModel.CreatingInvoice.User.ShouldBeNull();
			ViewModel.CreatingInvoice.Products.ShouldBeNull();
		}

		[TestMethod]
		public void InvoiceViewModel_SaveInvoice_HandlesExceptionWhenSelectedUserNull()
		{

			var products = Helpers.GenerateProducts(50);
			var users = Helpers.GenerateUsers(10, products);
			var selectedProducts = products.Where(p => p.ProductId % 3 == 0);

			ProductRepository.Setup(repo => repo.All()).Returns(products);
			UserRepository.Setup(repo => repo.All()).Returns(users);
			Logger.Setup(log => log.MinimumLevel).Returns(LogLevel.Warning);
			InvoiceRepository.Setup(repo => repo.Save(It.IsAny<Invoice>())).Throws(new ArgumentException("User", "User cannot be null"));

			ViewModel = new InvoiceViewModel(InvoiceRepository.Object, UserRepository.Object, ProductRepository.Object, Logger.Object);

			ViewModel.SaveInvoice();

			InvoiceRepository.Verify(repo => repo.Save(It.IsAny<Invoice>()), Times.Once());

			ViewModel.Errors.ShouldNotBeNull();
			ViewModel.Errors.ShouldNotBeEmpty();
		}

		[TestMethod]
		public void InvoiceViewModel_UserSelected_LoadsCommonProductsForUserThatHasInvoices()
		{
			var products = Helpers.GenerateProducts(15);
			var user = Helpers.GenerateUsers(1, products).First();

			var invoices = Enumerable.Empty<Invoice>();
			InvoiceRepository.Setup(repo => repo.HasInvoices(user.UserId, out invoices)).Returns(false);

			ViewModel.SelectedUser = user;

			ViewModel.UserSelected();

			ViewModel.CanShowUserInvoices.ShouldBeTrue();
			ViewModel.PreviouslyUsedProducts.ShouldNotBeEmpty();
			ViewModel.PreviouslyUsedProducts.ShouldAllBe(p => products.Contains(p));
		}

		[TestMethod]
		public void InvoiceViewModel_UserSelected_HidesProductsForUserWithoutInvoices()
		{
			var user = Helpers.GenerateUsers(1).First();

			ViewModel.SelectedUser = user;

			ViewModel.UserSelected();

			ViewModel.CanShowUserInvoices.ShouldBeFalse();
		}
	}
}
