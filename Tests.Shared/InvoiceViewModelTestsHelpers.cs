using MockingModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests.Shared
{
	public class InvoiceViewModelTestsHelpers
	{
		/// <summary>
		/// Generates a collection of users without their invoices
		/// </summary>
		/// <param name="quantity">The number of users to generate</param>
		/// <returns></returns>
		public List<User> GenerateUsers(int quantity)
		{
			var users = new List<User>();

			for (int i = 0; i < quantity; ++i)
			{
				var user = new User
				{
					Email = $"useremail{i}@mycompany.com",
					PasswordHash = new string((char)(i + 48), 32),
					UserId = i,
					Username = $"User {i}"
				};

				users.Add(user);
			}

			return users;
		}

		/// <summary>
		/// Generates a collection of users with their invoices. Invoices use the products collection to add products
		/// </summary>
		/// <param name="quantity">The number of users to generate</param>
		/// <param name="products">Products that should be included in the users' invoices</param>
		/// <returns></returns>
		public List<User> GenerateUsers(int quantity, IEnumerable<Product> products)
		{
			var users = new List<User>();

			for (int i = 0; i < quantity; ++i)
			{
				var user = new User
				{
					Email = $"useremail{i}@mycompany.com",
					PasswordHash = new string((char)(i + 48), 32),
					UserId = i,
					Username = $"User {i}"
				};

				var invoices = Enumerable.Range(0, i + 1)
				.Select(j => new Invoice
				{
					InvoiceId = i + j,
					User = user,
					Products = products.Take(i % products.Count()).ToList()
				}).ToList();

				user.Invoices = invoices;

				users.Add(user);
			}

			return users;
		}

		public List<Product> GenerateProducts(int quantity)
		{
			var products = new List<Product>();

			for (int i = 0; i < quantity; ++i)
			{
				products.Add(new Product
				{
					Name = $"Product {i}",
					Price = (i + 1) * 2.7m,
					ProductId = i,
					QuantityOnHand = (i * 3) % 17
				});
			}

			return products;
		}
	}
}
