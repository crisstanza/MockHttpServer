using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using System.Text;

namespace server.response
{
	class OutputBody
	{
		private static readonly Dictionary<string, string> contentTypeByExtension = new Dictionary<string, string>();

		public string File { get; set; }
		public byte[] Body { get; set; }
		public string ContentType { get; set; }

		static OutputBody()
		{
			contentTypeByExtension.Add(".html", MediaTypeNames.Text.Html);
			contentTypeByExtension.Add(".json", MediaTypeNames.Application.Json);
			contentTypeByExtension.Add(".ico", "image/x-icon");
			contentTypeByExtension.Add(".txt", MediaTypeNames.Text.Plain);
		}

		public OutputBody(string file, string body)
		{
			this.File = file;
			this.Body = Encoding.UTF8.GetBytes(body ?? "");
			SetContentType();
		}

		public OutputBody(string file, byte[] body)
		{
			this.File = file;
			this.Body = body;
			SetContentType();
		}

		private void SetContentType()
		{
			string extension = Path.GetExtension(this.File);
			if (extension != null && extension != "")
			{
				if (contentTypeByExtension.ContainsKey(extension))
				{
					this.ContentType = contentTypeByExtension[extension];
				}
				else
				{
					Console.WriteLine("[INFO] unmapped extension: " + extension);
					Console.WriteLine();
				}
			}
			else
			{
				this.ContentType = null;
			}
		}
	}
}
