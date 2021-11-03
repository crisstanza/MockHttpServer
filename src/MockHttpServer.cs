using servers;
using System;
using System.Reflection;
using utils;

class MockHttpServer
{
	private static readonly String DEFAULT_HOST = "localhost";
	private static readonly int DEFAULT_PORT = EnvironmentUtils.Variable("DEFAULT_PORT", 8910);

	static void Main(String[] args)
	{
		Console.WriteLine("Current version: " + Assembly.GetExecutingAssembly().GetName().Version);
		Console.WriteLine();
		int port = DEFAULT_PORT;
		for (int i = 0; i < args.Length; i++)
		{
			String arg = args[i];
			if (arg == "-port")
			{
				port = Int32.Parse(args[++i]);
			}
		}
		MainHttpServer server = new MainHttpServer(DEFAULT_HOST, port);
		server.Start();

	}
}
