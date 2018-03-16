using System;
using System.Collections.Generic;
using System.Text;

namespace MockingModels
{
	public class Invoice
	{
		public virtual User User { get; set; }
		public virtual ICollection<Product> Products { get; set; }
	}
}
