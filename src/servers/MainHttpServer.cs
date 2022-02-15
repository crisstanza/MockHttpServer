using server.response;
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
			Console.WriteLine("= cores: " + Environment.GetEnvironmentVariable("NUMBER_OF_PROCESSORS"));
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
						IAsyncResult asyncResult = this.mainHttpListener.BeginGetContext(new AsyncCallback(MainHttpListenerCallback), this.mainHttpListener);
						asyncResult.AsyncWaitHandle.WaitOne();
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
					SendResponse(context, new OutputBody(icon, contents));
					return;
				}
				else if (segment1 == "stop")
				{
					PrintInputInfo(request, inputBody);
					SendResponse(context, GetOutputBody(request, inputBody));
					this.mainHttpListener.Stop();
					return;
				}
			}
			PrintInputInfo(request, inputBody);
			SendResponse(context, GetOutputBody(request, inputBody));
		}

		private OutputBody GetOutputBody(HttpListenerRequest request, string inputBody)
		{
			string accept = request.Headers.Get("Accept");
			string ping = request.QueryString.Get("ping");
			if (ping != null)
			{
				String page = ping;
				String pagePath = this.pongPath + Path.DirectorySeparatorChar + page;
				String contents;
				if (File.Exists(pagePath))
				{
					contents = File.ReadAllText(pagePath);
					contents = contents.Replace("${version}", Assembly.GetExecutingAssembly().GetName().Version.ToString());
				}
				else
				{
					contents = "file not found: " + pagePath;
				}
				return new OutputBody(ping, contents, accept);
			}
			else
			{
				return new OutputBody(request.Url.ToString(), inputBody, accept);
			}
		}

		private void SendResponse(HttpListenerContext context, OutputBody output)
		{
			this.httpListenerUtils.Write(context.Response, output.ContentType, output.Body);
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
			Console.WriteLine("input body: " + inputBody);
			Console.WriteLine("================================================================================");
			Console.WriteLine();
		}

	}

}
