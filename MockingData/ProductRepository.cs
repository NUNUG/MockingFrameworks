using MockingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MockingData
{
	public class ProductRepository : IProductRepository
	{
		internal static List<Product> FakeProductData { get; set; }

		static ProductRepository()
		{
			FakeProductData = new List<Product>();
			var random = new Random();

			for(int i = 0; i < 100; ++i)
			{
				FakeProductData.Add(new Product
				{
					Name = $"Product {i + 1}",
					Price = (decimal)(Math.Round(random.NextDouble() * 100) / 10),
					ProductId = i,
					QuantityOnHand = random.Next(0, i * 5)
				});
			}
		}

		public IEnumerable<Product> All() => FakeProductData.AsEnumerable();
		public Product Find(int key) => FakeProductData.FirstOrDefault(p => p.ProductId == key);
		public IEnumerable<Product> Find(int key, params int[] keys) => FakeProductData.Where(p => p.ProductId == key || (keys != null && keys.Contains(p.ProductId)));
		public Product Save(Product entity)
		{
			var savedEntity = Find(entity.ProductId);

			if(savedEntity == null)
			{
				entity.ProductId = FakeProductData.Count;
				FakeProductData.Add(entity);
			}
			else
			{
				savedEntity.Name = entity.Name;
				savedEntity.Price = entity.Price;
				savedEntity.QuantityOnHand = entity.QuantityOnHand;
			}

			return entity;
		}
	}
}
