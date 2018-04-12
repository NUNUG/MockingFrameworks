using System;
using System.Collections.Generic;
using System.Text;

namespace Tests.Shared
{
	public class LambdaComparer<T> : IComparer<T>
	{
		public Func<T, T, int> Comparer { get; }

		public LambdaComparer(Func<T, T, int> comparer) => Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));


		public int Compare(T x, T y)
		{
			return Comparer(x, y);
		}
	}
}
