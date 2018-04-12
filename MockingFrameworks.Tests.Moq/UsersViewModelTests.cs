using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MockingFramework.Pages.Users;
using MockingModels;
using Moq;
using Shouldly;
using Tests.Shared;

namespace MockingFrameworks.Tests.Moq
{
	[TestClass]
	public class UsersViewModelTests
	{
		protected UsersViewModel ViewModel { get; set; }
		protected Mock<IUserRepository> UserRepository { get; set; }
		protected InvoiceViewModelTestsHelpers Helpers { get; set; }

		[TestInitialize]
		public void Initialize()
		{
			UserRepository = new Mock<IUserRepository>();
			Helpers = new InvoiceViewModelTestsHelpers();
		}

		[TestMethod]
		public void UserViewModel_FilterBy_FiltersByEmail()
		{
			// Setup
			var users = Helpers.GenerateUsers(10);

			UserRepository.Setup(u => u.All()).Returns(users);
			ViewModel.Filter = UsersViewModel.Filters.Email;

			ViewModel = new UsersViewModel(UserRepository.Object);

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

			UserRepository.Setup(u => u.All()).Returns(users);
			ViewModel.Filter = UsersViewModel.Filters.Username;

			ViewModel = new UsersViewModel(UserRepository.Object);


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

			UserRepository.Setup(u => u.All()).Returns(users);
			ViewModel.Filter = UsersViewModel.Filters.Id;

			ViewModel = new UsersViewModel(UserRepository.Object);


			ViewModel.Users.ShouldNotBeNull();
			ViewModel.Users.Count.ShouldBe(10);

			// Act
			ViewModel.FilterBy();

			ViewModel.Users.ShouldBeInOrder(SortDirection.Ascending, new LambdaComparer<User>((x, y) => x.UserId.CompareTo(y.UserId)));
		}
	}
}
