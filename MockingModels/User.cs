using System;
using System.Collections.Generic;
using System.Text;

namespace MockingModels
{
	public class User
	{
		public int UserId { get; set; }
		public string Username { get; set; }
		public string Email { get; set; }
		public string PasswordHash { get; set; }

		public virtual ICollection<Invoice> Invoices { get; set; }
	}
}
