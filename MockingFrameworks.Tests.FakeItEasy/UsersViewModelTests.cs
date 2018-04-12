using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MockingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using System.Linq.Expressions;
using Tests.Shared;
using MockingFrameworks.Pages.Users;

namespace MockingFrameworks.Tests.FakeItEasy
{
	[TestClass]
	public class UsersViewModelTests
	{
		protected UsersViewModel ViewModel { get; set; }
		protected IUserRepository UserRepository { get; set; }

		[TestInitialize]
		public void Initialize()
		{
			UserRepository = A.Fake<IUserRepository>();
			ViewModel = new UsersViewModel(UserRepository);
		}

		[TestMethod]
		public void UserViewModel_FilterBy_FiltersByEmail()
		{
			// Setup
			var users = A.CollectionOfDummy<User>(10);

			A.CallTo(() => UserRepository.All()).Returns(users);
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
			var users = A.CollectionOfDummy<User>(10);

			A.CallTo(() => UserRepository.All()).Returns(users);
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
			var users = A.CollectionOfDummy<User>(10);

			A.CallTo(() => UserRepository.All()).Returns(users);
			ViewModel.Filter = UsersViewModel.Filters.Id;
			ViewModel.Users.ShouldNotBeNull();
			ViewModel.Users.Count.ShouldBe(10);

			// Act
			ViewModel.FilterBy();

			ViewModel.Users.ShouldBeInOrder(SortDirection.Ascending, new LambdaComparer<User>((x, y) => x.UserId.CompareTo(y.UserId)));
		}
	}
}
