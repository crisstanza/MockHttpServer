using System;
using System.IO;
using System.Reflection;

namespace utils
{
	class FileSystemUtils
	{
		internal string CurrentPath()
		{
			return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + Path.DirectorySeparatorChar;
		}
	}
}
