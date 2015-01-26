using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Server.Interfaces;

namespace Server.Network
{
	public class TcpServer : ITransportServer
	{
		#region Fileds

		private readonly TcpListener tcpListener;

		private bool isStarted;

		private readonly List<IClient> clients = new List<IClient>(); 

		private readonly object syncClients = new object();

		#endregion

		#region Init/Deinit

		public TcpServer()
		{
			tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 3333);
		}

		public void Dispose()
		{
			try
			{
				isStarted = false;
				tcpListener.Stop();
				foreach (var client in clients)
				{
					client.Disconnect -= ClientOnDisconnect;
					client.Dispose();
				}
			}
			catch
			{

			}
		}

		#endregion

		#region Public methods

		public void Start()
		{
			isStarted = true;
			tcpListener.Start();
			ListenForClients();
		}

		public List<IClient> GetClients()
		{
			lock (syncClients)
			{
				return clients.ToList();
			}
		}

		#endregion

		#region Private methods

		private async void ListenForClients()
		{

			while (isStarted)
			{
				var tcpClient = await tcpListener.AcceptTcpClientAsync();
				Console.WriteLine("Client connected");
				var client = new TcpClient(tcpClient);
				client.Disconnect += ClientOnDisconnect;
				lock (syncClients)
				{
					clients.Add(client);
				}
				RaiseClientConnected(client);
			}
		}

		private void ClientOnDisconnect(IClient client)
		{
			lock (syncClients)
			{
				clients.Remove(client);
			}
			Console.WriteLine("Client disconnected");
		}


		private void RaiseClientConnected(IClient client)
		{
			var handler = OnNewClientConnected;
			if (handler != null) handler(client);
		}

		#endregion

		public event Action<IClient> OnNewClientConnected;
		
	}
}
