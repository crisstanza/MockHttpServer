using System;
using System.IO;
using System.Text;

namespace utils
{
	class StreamUtils
	{
		public String ReadToEnd(Stream inputStream, Encoding contentEncoding)
		{
			StreamReader reader = new StreamReader(inputStream, contentEncoding);
			String all = reader.ReadToEnd();
			reader.Close();
			return all;
		}
	}
}
