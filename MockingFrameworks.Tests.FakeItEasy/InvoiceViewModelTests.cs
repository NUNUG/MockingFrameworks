using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MockingFramework.Pages.Invoice;
using MockingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Shared;
using Shouldly;

namespace MockingFrameworks.Tests.FakeItEasy
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

			InvoiceRepository = A.Fake<IInvoiceRepository>();
			UserRepository = A.Fake<IUserRepository>();
			ProductRepository = A.Fake<IProductRepository>();
			Logger = A.Fake<ILog>();

			ViewModel = new InvoiceViewModel(InvoiceRepository, UserRepository, ProductRepository, Logger);
		}

		[TestMethod]
		public void InvoiceViewModel_SaveInvoice_SavesInvoiceWithCorrectData()
		{
			var products = A.CollectionOfDummy<Product>(50);
			var users = A.CollectionOfDummy<User>(10);
			var selectedProducts = products.Where(p => p.ProductId % 3 == 0);
			var selectedUser = users.ElementAt(2);

			A.CallTo(() => ProductRepository.All()).Returns(products);
			A.CallTo(() => UserRepository.All()).Returns(users);
			A.CallTo(() => InvoiceRepository.Save(A<Invoice>.Ignored)).WithAnyArguments().ReturnsLazily(fake => fake.Arguments.First() as Invoice);
			A.CallTo(() => Logger.MinimumLevel).Returns(LogLevel.Warning);

			foreach (var product in selectedProducts)
				ViewModel.CreatingInvoice.Products.Add(product);

			ViewModel.SelectedUser = selectedUser;
			ViewModel.CreatingInvoice.Products.ShouldBeOneOf(selectedProducts.ToArray());

			ViewModel.SaveInvoice();

			A.CallTo(() => InvoiceRepository.Save(A<Invoice>.That.Matches(i => i.User == selectedUser))).MustHaveHappenedOnceExactly();
			A.CallTo(() => Logger.MinimumLevel).MustHaveHappenedOnceExactly();
			A.CallTo(() => Logger.Debug(A<string>.Ignored)).MustNotHaveHappened();
			A.CallTo(() => Logger.Information(A<string>.Ignored)).MustNotHaveHappened();
			A.CallTo(() => Logger.Warning(A<string>.Ignored)).MustNotHaveHappened();
			A.CallTo(() => Logger.Error(A<string>.Ignored)).MustNotHaveHappened();

			ViewModel.Errors.ShouldBeNull();
			ViewModel.CreatingInvoice.User.ShouldBeNull();
			ViewModel.CreatingInvoice.Products.ShouldBeNull();
		}

		[TestMethod]
		public void InvoiceViewModel_SaveInvoice_DoesntSaveInvoiceWithIncorrectData()
		{
			var products = A.CollectionOfDummy<Product>(50);
			var users = A.CollectionOfDummy<User>(10);
			var selectedProducts = products.Where(p => p.ProductId % 3 == 0);
			var selectedUser = users.ElementAt(2);

			A.CallTo(() => ProductRepository.All()).Returns(products);
			A.CallTo(() => UserRepository.All()).Returns(users);
			A.CallTo(() => Logger.MinimumLevel).Returns(LogLevel.Warning);

			ViewModel.SelectedUser = selectedUser;

			ViewModel.SaveInvoice();

			ViewModel.Errors.ShouldNotBeEmpty();
			A.CallTo(() => Logger.Warning(A<string>.Ignored)).MustHaveHappenedOnceExactly();
			A.CallTo(() => Logger.Debug(A<string>.Ignored)).MustNotHaveHappened();
			A.CallTo(() => Logger.Information(A<string>.Ignored)).MustNotHaveHappened();
			A.CallTo(() => Logger.Error(A<string>.Ignored)).MustNotHaveHappened();
			A.CallTo(() => InvoiceRepository.Save(A<Invoice>.Ignored)).MustNotHaveHappened();

			foreach (var product in selectedProducts)
				ViewModel.CreatingInvoice.Products.Add(product);

			ViewModel.CreatingInvoice.Products.ShouldBeOneOf(selectedProducts.ToArray());

			ViewModel.SaveInvoice();

			A.CallTo(() => Logger.Warning(A<string>.Ignored)).MustHaveHappenedTwiceExactly();
			A.CallTo(() => Logger.Debug(A<string>.Ignored)).MustNotHaveHappened();
			A.CallTo(() => Logger.Information(A<string>.Ignored)).MustNotHaveHappened();
			A.CallTo(() => Logger.Error(A<string>.Ignored)).MustNotHaveHappened();
			A.CallTo(() => InvoiceRepository.Save(A<Invoice>.That.Matches(i => i.User == selectedUser))).MustHaveHappenedOnceExactly();

			ViewModel.CreatingInvoice.User.ShouldBeNull();
			ViewModel.CreatingInvoice.Products.ShouldBeNull();
		}

		[TestMethod]
		public void InvoiceViewModel_SaveInvoice_HandlesExceptionWhenSelectedUserNull()
		{

			var products = Helpers.GenerateProducts(50);
			var users = Helpers.GenerateUsers(10, products);
			var selectedProducts = products.Where(p => p.ProductId % 3 == 0);

			A.CallTo(() => ProductRepository.All()).Returns(products);
			A.CallTo(() => UserRepository.All()).Returns(users);
			A.CallTo(() => Logger.MinimumLevel).Returns(LogLevel.Warning);
			A.CallTo(() => InvoiceRepository.Save(A<Invoice>.Ignored)).Throws(new ArgumentException("User", "User cannot be null"));

			ViewModel.SaveInvoice();

			A.CallTo(() => InvoiceRepository.Save(A<Invoice>.Ignored)).MustHaveHappenedOnceExactly();

			ViewModel.Errors.ShouldNotBeNull();
			ViewModel.Errors.ShouldNotBeEmpty();
		}

		[TestMethod]
		public void InvoiceViewModel_UserSelected_LoadsCommonProductsForUserThatHasInvoices()
		{
			var products = A.CollectionOfDummy<Product>(50);
			var user = A.Dummy<User>();
			IEnumerable<Invoice> invoices;

			A.CallTo(() => InvoiceRepository.HasInvoices(user.UserId, out invoices))
			.Returns(false)
			.AssignsOutAndRefParameters(new List<Invoice>());

			A.CallTo(() => InvoiceRepository.HasInvoices(user.UserId, out invoices))
			.Returns(false)
			.AssignsOutAndRefParametersLazily(fake => new[] { new List<Invoice>() });

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
