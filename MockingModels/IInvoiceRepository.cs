using System;
using System.Collections.Generic;
using System.Text;

namespace MockingModels
{
	public interface IInvoiceRepository : IRepository<Invoice, int>
	{
		IEnumerable<Invoice> FindForUser(int userId);
		IEnumerable<Invoice> FindForUser(string username);
		bool HasInvoices(int userId, out IEnumerable<Invoice> invoices);
	}
}
