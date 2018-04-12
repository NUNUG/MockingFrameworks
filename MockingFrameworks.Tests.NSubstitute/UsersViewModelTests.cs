
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MockingFramework.Pages.Users;
using MockingModels;
using NSubstitute;
using Shouldly;
using Tests.Shared;

namespace MockingFrameworks.Tests.NSubstitute
{
	[TestClass]
	public class UsersViewModelTests
	{
		protected UsersViewModel ViewModel { get; set; }
		protected IUserRepository UserRepository { get; set; }
		protected InvoiceViewModelTestsHelpers Helpers { get; set; }

		[TestInitialize]
		public void Initialize()
		{
			UserRepository = Substitute.For<IUserRepository>();
			ViewModel = new UsersViewModel(UserRepository);
			Helpers = new InvoiceViewModelTestsHelpers();
		}

		[TestMethod]
		public void UserViewModel_FilterBy_FiltersByEmail()
		{
			// Setup
			var users = Helpers.GenerateUsers(10);

			UserRepository.All().Returns(users);
			ViewModel.Filter = UsersViewModel.Filters.Email;
			ViewModel.Users.ShouldNotBeNull();
			ViewModel.Users.Count.ShouldBe(10);

			// Act
			ViewModel.FilterBy();

			ViewModel.Users.ShouldBeInOrder(SortDirection.Ascending, new LambdaComparer<User>((x, y) => x.Email.CompareTo(y.Email)));
		}

		[TestMethod]
		public void UserViewModel_FilterBy_FiltersByUsername()
		{
			// Setup
			var users = Helpers.GenerateUsers(10);

			UserRepository.All().Returns(users);
			ViewModel.Filter = UsersViewModel.Filters.Username;
			ViewModel.Users.ShouldNotBeNull();
			ViewModel.Users.Count.ShouldBe(10);

			// Act
			ViewModel.FilterBy();

			ViewModel.Users.ShouldBeInOrder(SortDirection.Ascending, new LambdaComparer<User>((x, y) => x.Username.CompareTo(y.Username)));
		}

		[TestMethod]
		public void UserViewModel_FilterBy_FiltersById()
		{
			// Setup
			var users = Helpers.GenerateUsers(10);

			UserRepository.All().Returns(users);
			ViewModel.Filter = UsersViewModel.Filters.Id;
			ViewModel.Users.ShouldNotBeNull();
			ViewModel.Users.Count.ShouldBe(10);

			// Act
			ViewModel.FilterBy();

			ViewModel.Users.ShouldBeInOrder(SortDirection.Ascending, new LambdaComparer<User>((x, y) => x.UserId.CompareTo(y.UserId)));
		}
	}
}
