using System;
using System.Collections.Generic;
using System.Text;

namespace MockingModels
{
	public interface IInvoiceRepository : IRepository<Product, int>
	{
		IEnumerable<Product> FindForUser(int userId);
		IEnumerable<Product> FindForUser(string username);
	}
}
