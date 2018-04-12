using System;
using System.Collections.Generic;
using System.Text;

namespace MockingModels
{
	public static class IEnumerableExtensions
	{
		public static IEnumerable<T> ForEach<T>(this IEnumerable<T> self, Action<T> callback)
		{
			foreach (var t in self)
				callback?.Invoke(t);

			return self;
		}

		public static IEnumerable<T> ForEach<T>(this IEnumerable<T> self, Predicate<T> callback)
		{
			foreach(var t in self)
			{
				var rtVal = callback?.Invoke(t);

				if (rtVal == true)
					break;
			}

			return self;
		}
	}
}
