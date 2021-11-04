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

		public string Accept { get; set; }

		static OutputBody()
		{
			contentTypeByExtension.Add(".html", MediaTypeNames.Text.Html);
			contentTypeByExtension.Add(".json", MediaTypeNames.Application.Json);
			contentTypeByExtension.Add(".ico", "image/x-icon");
			contentTypeByExtension.Add(".txt", MediaTypeNames.Text.Plain);
			contentTypeByExtension.Add(".xml", MediaTypeNames.Text.Xml);
		}

		public OutputBody(string file, string body, string accept)
		{
			this.File = file;
			this.Body = Encoding.UTF8.GetBytes(body ?? "");
			this.Accept = accept;
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
				string extensionLower = extension.ToLower();
				if (contentTypeByExtension.ContainsKey(extensionLower))
				{
					this.ContentType = contentTypeByExtension[extensionLower];
					return;
				}
				else
				{
					Console.WriteLine("[INFO] unmapped extension: " + extension);
					Console.WriteLine();
				}
			}
			if (Accept != null && Accept != "")
			{
				this.ContentType = Accept.Split(",")[0];
			}
		}
	}
}
