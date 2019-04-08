using System.Collections.Generic;

namespace Utils
{
	public static class LinqUtils
	{
		public static bool TryGetFirstValue<T>(this IEnumerable<T> self, out T value)
		{
			foreach (T t in self)
			{
				value = t;
				return true;
			}

			value = default(T);
			return false;
		}
	}
}