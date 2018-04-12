using Caliburn.Micro;
using MockingFramework.Models;
using MockingModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MockingFramework.Pages.Users
{
	public class UsersViewModel : IconScreen
	{
		public enum Filters
		{
			Email,
			Username,
			Id
		}


		private ObservableCollection<User> _Users;
		public ObservableCollection<User> Users
		{
			get => _Users;
			protected set { _Users = value; NotifyOfPropertyChange(nameof(Users)); }
		}
		private Filters _Filter;
		public Filters Filter
		{
			get => _Filter;
			set { _Filter = value; NotifyOfPropertyChange(nameof(Filter)); NotifyOfPropertyChange(nameof(Users)); }
		}

		protected IUserRepository UserRepository { get; }

		public UsersViewModel(IUserRepository userRepository)
		{
			UserRepository = userRepository;
			Users = new ObservableCollection<User>(userRepository.All());

			DisplayName = "Users";
			Icon = "Refresh";
		}

		protected override void OnActivate()
		{
			base.OnActivate();

			Users = new ObservableCollection<User>(UserRepository.All());
		}

		public void FilterBy()
		{
			switch (Filter)
			{
				case Filters.Email:
					Users = new ObservableCollection<User>(Users.OrderBy(u => u.Email));
					break;
				case Filters.Id:
					Users = new ObservableCollection<User>(Users.OrderBy(u => u.UserId));
					break;
				case Filters.Username:
					Users = new ObservableCollection<User>(Users.OrderBy(u => u.Username));
					break;
			}
		}
	}
}
