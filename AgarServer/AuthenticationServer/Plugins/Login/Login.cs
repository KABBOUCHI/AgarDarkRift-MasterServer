using System;
using System.Linq;
using AuthenticationServer;
using AuthenticationServer.Models;
using DarkRift;
using DarkRift.Server;

namespace LoginPlugin
{
	public class Login : Plugin
	{
		public override Version Version => new Version(1, 0, 0);
		public override bool ThreadSafe => true;
		public override Command[] Commands => new[]
		{

			new Command("Online", "Logs number of online users", "", (e,arg) =>{})
		};

		private const ushort LoginUser = 0;
		private const ushort LogoutUser = 1;
		private const ushort AddUser = 2;
		private const ushort LoginSuccess = 3;
		private const ushort LoginFailed = 4;
		private const ushort LogoutSuccess = 5;
		private const ushort AddUserSuccess = 6;
		private const ushort AddUserFailed = 7;


		public Login(PluginLoadData pluginLoadData) : base(pluginLoadData)
		{
			ClientManager.ClientConnected += OnPlayerConnected;
			ClientManager.ClientDisconnected += OnPlayerDisconnected;
		}

		private void OnPlayerConnected(object sender, ClientConnectedEventArgs e)
		{
			e.Client.MessageReceived += OnMessageReceived;
		}

		private void OnPlayerDisconnected(object sender, ClientDisconnectedEventArgs e)
		{
		}

		private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
		{
			using (var message = e.GetMessage())
			{
				var client = e.Client;

				switch (message.Tag)
				{
					case LoginUser:

						using (var reader = message.GetReader())
						{
							var username = reader.ReadString();
							var password = reader.ReadString();

							User user = null;

							using (var context = new AuthenticationContext())
							{
								user = context
									.Users
									.FirstOrDefault(
										u => u.Username == username &&
										u.Password == password
									);
							}

							using (var msg = Message.CreateEmpty(user != null ? LoginSuccess : LoginFailed))
							{
								client.SendMessage(msg, SendMode.Reliable);
							}
						}
						break;
				}
			}
		}
	}
}
