using System;

namespace utils
{
	class EnvironmentUtils
	{
		public static bool Variable(string name, bool fallback)
		{
			string value = Environment.GetEnvironmentVariable(name);
			return value == null ? fallback : Boolean.Parse(value);
		}
		public static int Variable(string name, int fallback)
		{
			string value = Environment.GetEnvironmentVariable(name);
			return value == null ? fallback : Int32.Parse(value);
		}
		public static string Variable(string name, string fallback)
		{
			string value = Environment.GetEnvironmentVariable(name);
			return value == null ? fallback : value;
		}
	}
}
