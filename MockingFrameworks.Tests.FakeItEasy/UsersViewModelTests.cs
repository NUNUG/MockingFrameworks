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
using System.Threading;

namespace MockingFrameworks.Tests.FakeItEasy
{
	[TestClass]
	public class UsersViewModelTests
	{
		protected UsersViewModel ViewModel { get; set; }
		protected IUserRepository UserRepository { get; set; }
		protected CaliburnHelpers CaliburnHelpers { get; set; }

		[TestInitialize]
		public void Initialize()
		{
			UserRepository = A.Fake<IUserRepository>();
			ViewModel = new UsersViewModel(UserRepository);
			CaliburnHelpers = new CaliburnHelpers();
		}

		[TestMethod]
		public void UserViewModel_FilterBy_FiltersByEmail()
		{
			// Setup
			var users = A.CollectionOfDummy<User>(10);

			A.CallTo(() => UserRepository.All()).Returns(users);
			ViewModel.Filter = UsersViewModel.Filters.Email;

			CaliburnHelpers.ActivateViewModel(ViewModel);

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

			CaliburnHelpers.ActivateViewModel(ViewModel);

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

			CaliburnHelpers.ActivateViewModel(ViewModel);

			ViewModel.Users.ShouldNotBeNull();
			ViewModel.Users.Count.ShouldBe(10);

			// Act
			ViewModel.FilterBy();

			ViewModel.Users.ShouldBeInOrder(SortDirection.Ascending, new LambdaComparer<User>((x, y) => x.UserId.CompareTo(y.UserId)));
		}
	}

	internal class DummyUserFactory : DummyFactory<User>
	{
		private ReaderWriterLockSlim Lock { get; }
		protected int Counter { get; set; }

		public DummyUserFactory()
		{
			Counter = 0;
			Lock = new ReaderWriterLockSlim();
		}

		protected override User Create()
		{
			Lock.EnterUpgradeableReadLock();

			var user = new User
			{
				Email = $"useremail{Counter}",
				PasswordHash = new string((char)(Counter + 48), 16),
				UserId = Counter,
				Username = $"User {Counter}"
			};

			Lock.EnterWriteLock();
			++Counter;
			Lock.ExitWriteLock();

			Lock.ExitUpgradeableReadLock();
			return user;
		}
	}
}
