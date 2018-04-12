using MockingModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MockingData
{
	public class UserRepository : IUserRepository
	{
		internal static List<User> FakeUserData { get; set; }

		static UserRepository()
		{
			FakeUserData = new List<User>();

			for(int i = 0; i < 100; ++i)
			{
				FakeUserData.Add(new User
				{
					Email = $"email{i + 1}@company.com",
					PasswordHash = $"password{i}",
					UserId = i,
					Username = $"user_{i}"
				});
			}
		}

		public IEnumerable<User> All() => FakeUserData.AsEnumerable();
		public User Find(int key) => FakeUserData.FirstOrDefault(u => u.UserId == key);
		public IEnumerable<User> Find(int key, params int[] keys) => FakeUserData.Where(u => u.UserId == key || (keys != null && keys.Contains(u.UserId)));
		public User FindByEmail(string email) => FakeUserData.FirstOrDefault(u => u.Email == email);
		public User FindByUsername(string username) => FakeUserData.FirstOrDefault(u => u.Username == username);
		public User Save(User entity)
		{
			var storedEntity = Find(entity.UserId);

			if(storedEntity == null)
			{
				FakeUserData.Add(entity);
				entity.UserId = FakeUserData.Count - 1;
			}
			else
			{
				storedEntity.Email = entity.Email;
				storedEntity.PasswordHash = entity.PasswordHash;
				storedEntity.Username = entity.Username;
			}

			return entity;
		}
	}
}
