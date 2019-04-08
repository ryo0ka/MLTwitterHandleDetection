using System;

namespace Utils
{
	public static class LangUtils
	{
		public static TimeSpan Seconds(this float seconds)
		{
			return TimeSpan.FromSeconds(seconds);
		}
	}
}