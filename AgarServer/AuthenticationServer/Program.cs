﻿using System;
using System.Collections.Generic;

using DarkRift.Server;
using System.Threading;
using System.IO;
using System.Collections.Specialized;
using LoginPlugin;

namespace AuthenticationServer
{
	class Program
	{
		/// <summary>
		///     The server instance.
		/// </summary>
		static DarkRiftServer server;


		protected static int serverTickRate = 4;

		public static event EventHandler<IClientManager> OnTick;

		/// <summary>
		///     Main entry point of the server which starts a single server.
		/// </summary>
		/// <param name="args"></param>
		public static void Main(string[] args)
		{
			string[] rawArguments = CommandEngine.ParseArguments(string.Join(" ", args));
			string[] arguments = CommandEngine.GetArguments(rawArguments);
			NameValueCollection variables = CommandEngine.GetFlags(rawArguments);

			string configFile;
			if (arguments.Length == 0)
			{
				configFile = "Server.config";
			}
			else if (arguments.Length == 1)
			{
				configFile = arguments[0];
			}
			else
			{
				System.Console.Error.WriteLine("Invalid comand line arguments.");
				System.Console.WriteLine("Press any key to exit...");
				System.Console.ReadKey();
				return;
			}

			ServerSpawnData spawnData;

			try
			{
				spawnData = ServerSpawnData.CreateFromXml(configFile, variables);
			}
			catch (IOException e)
			{
				System.Console.Error.WriteLine("Could not load the config file needed to start (" + e.Message + "). Are you sure it's present and accessible?");
				System.Console.WriteLine("Press any key to exit...");
				System.Console.ReadKey();
				return;
			}
			catch (XmlConfigurationException e)
			{
				System.Console.Error.WriteLine(e.Message);
				System.Console.WriteLine("Press any key to exit...");
				System.Console.ReadKey();
				return;
			}
			catch (KeyNotFoundException e)
			{
				System.Console.Error.WriteLine(e.Message);
				System.Console.WriteLine("Press any key to exit...");
				System.Console.ReadKey();
				return;
			}

			spawnData.PluginSearch.PluginTypes.Add(typeof(Login));

			server = new DarkRiftServer(spawnData);

			server.Start();


			new Thread(new ThreadStart(ConsoleLoop)).Start();

			while (true)
			{
				//server.DispatcherWaitHandle.WaitOne();
				Thread.Sleep(1000 / serverTickRate);
				server.ExecuteDispatcherTasks();
				OnTick?.Invoke(null, server.ClientManager);
			}
		}

		/// <summary>
		///     Invoked from another thread to repeatedly execute commands from the console.
		/// </summary>
		static void ConsoleLoop()
		{
			while (true)
			{
				string input = System.Console.ReadLine();

				server.ExecuteCommand(input);
			}
		}
	}
}
