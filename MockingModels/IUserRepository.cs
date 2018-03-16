using System;
using System.Collections.Generic;
using System.Text;

namespace MockingModels
{
	public interface IUserRepository : IRepository<User, int>
	{
		User FindByUsername(string username);
		User FindByEmail(string email);
	}
}
