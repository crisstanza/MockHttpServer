using System.IO;
using System.Net;
using System.Text;

namespace utils
{
	class HttpListenerUtils
	{
		public void Write(HttpListenerResponse response, string contentType, byte[] buffer)
		{
			if (contentType != null)
			{
				response.ContentType = contentType;
			}
			response.ContentLength64 = buffer.Length;
			response.StatusCode = (int)HttpStatusCode.OK;
			Stream output = response.OutputStream;
			try
			{
				output.Write(buffer, 0, buffer.Length);
			}
			finally
			{
				output.Close();
			}
		}
	}
}
