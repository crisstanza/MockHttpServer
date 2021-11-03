using System.IO;
using System.Net;
using System.Text;

namespace utils
{
	class HttpListenerUtils
	{
		public void Write(HttpListenerResponse response, string contentType, string responseString)
		{
			byte[] buffer = Encoding.UTF8.GetBytes(responseString == null ? "" : responseString);
			Write(response, contentType, buffer);
		}

		public void Write(HttpListenerResponse response, string contentType, byte[] buffer)
		{
			if (contentType != null)
			{
				response.ContentType = contentType;
			}
			response.ContentLength64 = buffer.Length;
			response.StatusCode = (int)HttpStatusCode.OK;
			Stream output = response.OutputStream;
			output.Write(buffer, 0, buffer.Length);
			output.Close();
		}
	}
}
