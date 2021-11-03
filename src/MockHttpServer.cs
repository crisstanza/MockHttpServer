using servers;
using System;
using System.IO;
using System.Reflection;
using utils;

class MockHttpServer
{
	private static readonly String DEFAULT_HOST = "localhost";
	private static readonly int DEFAULT_PORT = EnvironmentUtils.Variable("DEFAULT_PORT", 8910);

	static void Main(String[] args)
	{
		FileSystemUtils fileSystemUtils = new FileSystemUtils(); 
		Console.WriteLine("Current version: " + Assembly.GetExecutingAssembly().GetName().Version);
		Console.WriteLine();
		int port = DEFAULT_PORT;
		string pongPath = fileSystemUtils.CurrentPath() + "html";
		for (int i = 0; i < args.Length; i++)
		{
			String arg = args[i];
			if (arg == "-port")
			{
				port = Int32.Parse(args[++i]);
			}
			else if (arg == "-pong")
			{
				pongPath = args[++i];
			}
		}
		MainHttpServer server = new MainHttpServer(DEFAULT_HOST, port, pongPath);
		server.Start();

	}
}
