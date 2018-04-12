using MockingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MockingData
{
	public class InvoiceRepository : IInvoiceRepository
	{
		internal static List<Invoice> FakeInvoiceData { get; set; }
		protected static bool IsInitiated { get; set; }
		protected static readonly ReaderWriterLockSlim Lock = new ReaderWriterLockSlim();

		internal static void Init()
		{
			FakeInvoiceData = new List<Invoice>();

			for(int i = 0; i < 5; ++i)
			{
				FakeInvoiceData.Add(new Invoice
				{
					InvoiceId = i,
					User = UserRepository.FakeUserData.FirstOrDefault(u => u.UserId == i),
					Products = ProductRepository.FakeProductData.Where(p => p.ProductId <= i + 1).ToList()
				});
			}
		}

		public InvoiceRepository()
		{
			if (!IsInitiated)
			{
				Lock.EnterWriteLock();
				Init();
				IsInitiated = true;
				Lock.ExitWriteLock();
			}
		}

		public IEnumerable<Invoice> All() => FakeInvoiceData.AsEnumerable();
		public Invoice Find(int key) => FakeInvoiceData.FirstOrDefault(i => i.InvoiceId == key);
		public IEnumerable<Invoice> Find(int key, params int[] keys) => FakeInvoiceData.Where(i => i.InvoiceId == key || (keys != null && keys.Contains(i.InvoiceId)));
		public IEnumerable<Invoice> FindForUser(int userId) => FakeInvoiceData.Where(i => i.User.UserId == userId);
		public IEnumerable<Invoice> FindForUser(string username) => FakeInvoiceData.Where(i => i.User.Username == username);
		public Invoice Save(Invoice entity)
		{
			Lock.EnterUpgradeableReadLock();

			var savedEntity = FakeInvoiceData.FirstOrDefault(i => i.InvoiceId == entity.InvoiceId);
			if(savedEntity == null)
			{
				entity.InvoiceId = FakeInvoiceData.Count;
				FakeInvoiceData.Add(entity);
			}
			else
			{
				savedEntity.Products = entity.Products;
				savedEntity.User = entity.User;
			}

			Lock.ExitUpgradeableReadLock();

			return entity;
		}
		public bool HasInvoices(int userId, out IEnumerable<Invoice> invoices)
		{
			var userInvoices = FakeInvoiceData.Where(i => i.User.UserId == userId);
			invoices = userInvoices;
			return userInvoices.Any();
		}
	}
}
