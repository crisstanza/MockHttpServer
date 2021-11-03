using System;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Reflection;
using System.Threading;
using utils;

namespace servers
{
	class MainHttpServer
	{
		private readonly string hostToListen;
		private readonly int portToListen;
		private readonly string pongPath;

		private readonly HttpListener mainHttpListener;
		
		private readonly HttpListenerUtils httpListenerUtils;
		private readonly StreamUtils streamUtils;
		private readonly DateTimeUtils dateTimeUtils;
		private readonly FileSystemUtils fileSystemUtils;

		public MainHttpServer(string hostToListen, int portToListen, string pongPath)
		{
			this.hostToListen = hostToListen;
			this.portToListen = portToListen;
			this.pongPath = pongPath;
			this.mainHttpListener = new HttpListener();
			this.httpListenerUtils = new HttpListenerUtils();
			this.streamUtils = new StreamUtils();
			this.dateTimeUtils = new DateTimeUtils();
			this.fileSystemUtils = new FileSystemUtils();
		}

		internal void Start()
		{
			string prefix = string.Format("http://{0}:{1}/", hostToListen, portToListen);
			this.mainHttpListener.Prefixes.Add(prefix);
			this.mainHttpListener.Start();
			Console.WriteLine("================================================================================");
			Console.WriteLine("= " + this.dateTimeUtils.Now());
			Console.WriteLine("= listening on: " + prefix);
			Console.WriteLine("= pong path: " + this.pongPath);
			Console.WriteLine("================================================================================");
			Console.WriteLine();
			new Thread(() =>
			{
				while (this.mainHttpListener.IsListening)
				{
					try
					{
						IAsyncResult context = this.mainHttpListener.BeginGetContext(new AsyncCallback(MainHttpListenerCallback), this.mainHttpListener);
						context.AsyncWaitHandle.WaitOne();
					}
					catch (HttpListenerException exc)
					{
						Console.WriteLine(exc.Message);
					}
				}
				Console.WriteLine("Server stop!");
			}).Start();
		}

		private void MainHttpListenerCallback(IAsyncResult result)
		{
			HttpListener listener = (HttpListener)result.AsyncState;
			if (listener.IsListening)
			{
				var context = listener.EndGetContext(result);
				ProcessRequest(context);
			}
		}

		private void ProcessRequest(HttpListenerContext context)
		{
			HttpListenerRequest request = context.Request;
			string inputBody = this.streamUtils.ReadToEnd(request.InputStream, request.ContentEncoding);
			string[] urlSegments = request.Url.Segments;
			if (urlSegments.Length == 2)
			{
				string segment1 = urlSegments[1];
				if (segment1 == "favicon.ico")
				{
					String currentPath = this.fileSystemUtils.CurrentPath();
					String icon = "favicon.ico";
					String iconPath = currentPath + "html" + Path.DirectorySeparatorChar + icon;
					byte[] contents = File.ReadAllBytes(iconPath);
					PrintInputInfo(request, inputBody);
					SendResponseIco(context, contents);
					return;
				}
				else if (segment1 == "stop")
				{
					PrintInputInfo(request, inputBody);
					SendResponseJson(context, inputBody);
					this.mainHttpListener.Stop();
					return;
				}
			}
			PrintInputInfo(request, inputBody);
			String outputBody = GetOutputBody(request, inputBody);
			SendResponseJson(context, outputBody);
		}

		private string GetOutputBody(HttpListenerRequest request, string inputBody)
		{
			string ping = request.QueryString.Get("ping");
			if (ping != null)
			{
				String currentPath = this.fileSystemUtils.CurrentPath();
				String page = "pong" + Path.DirectorySeparatorChar + ping;
				String pagePath = this.pongPath + Path.DirectorySeparatorChar + page;
				String contents = File.ReadAllText(pagePath);
				contents = contents.Replace("${version}", Assembly.GetExecutingAssembly().GetName().Version.ToString());
				return contents;
			}
			else
			{
				return inputBody;
			}
		}

		private void PrintInputInfo(HttpListenerRequest request, string inputBody)
		{
			Console.WriteLine("================================================================================");
			Console.WriteLine(this.dateTimeUtils.Now());
			Console.WriteLine();
			Console.WriteLine("url: " + request.Url);
			Console.WriteLine("method: " + request.HttpMethod);
			Console.WriteLine();
			Console.WriteLine("ping: " + request.QueryString.Get("ping"));
			Console.WriteLine();
			Console.WriteLine("headers: " + request.Headers);
			Console.WriteLine("body: " + inputBody);
			Console.WriteLine("================================================================================");
			Console.WriteLine();
		}

		private void SendResponseIco(HttpListenerContext context, byte[] responseArray)
		{
			SendResponseArray(context, "image/x-icon", responseArray);
		}

		private void SendResponseText(HttpListenerContext context, string responsestring)
		{
			SendResponse(context, MediaTypeNames.Text.Html, responsestring);
		}
		private void SendResponseJson(HttpListenerContext context, string responsestring)
		{
			SendResponse(context, MediaTypeNames.Application.Json, responsestring);
		}

		private void SendResponse(HttpListenerContext context, string contentType, string responsestring)
		{
			this.httpListenerUtils.Write(context.Response, contentType, responsestring);
		}

		private void SendResponseArray(HttpListenerContext context, String contentType, byte[] responseArray)
		{
			this.httpListenerUtils.Write(context.Response, contentType, responseArray);
		}

	}

}
