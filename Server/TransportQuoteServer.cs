using System;
using System.Collections.Generic;
using Server.Interfaces;
using Server.Models;

namespace Server
{
	public class TransportQuoteServer : IQuoteTransportServer
	{
		#region Fields

		private readonly Dictionary<IClient, QuoteFormat> quoteFormats = new Dictionary<IClient, QuoteFormat>();
		private readonly ITransportServer server;
		private bool isDisposed;

		#endregion

		#region Init/deinit

		public TransportQuoteServer(ITransportServer transportServer)
		{
			this.server = transportServer;
			server.OnNewClientConnected += ServerOnOnNewClientConnected;
		}

		public void Dispose()
		{
			if (isDisposed) return;
			try
			{
				isDisposed = true;
				var clients = server.GetClients();
				foreach (var client in clients)
				{
					client.ChangeQuoteFormat -= ClientOnChangeQuoteFormat;
				}
				server.OnNewClientConnected -= ServerOnOnNewClientConnected;
			}
			catch
			{

			}
		}

		#endregion

		#region Public methods

		public void SendToAll(IEnumerable<Quote> quotes)
		{
			foreach (var client in server.GetClients())
			{
				var format = QuoteFormat.Raw;
				if(quoteFormats.ContainsKey(client))
					format = quoteFormats[client];
				var data = quotes.Serialize(format == QuoteFormat.Column);
				client.SendData(data);
			}
		}

		public void Start()
		{
			server.Start();
		}

		#endregion

		#region Private methods

		private void ServerOnOnNewClientConnected(IClient client)
		{
			quoteFormats[client] = QuoteFormat.Raw;
			client.ChangeQuoteFormat += ClientOnChangeQuoteFormat;
			OnOnNewClientConnected(client);
		}

		private void ClientOnChangeQuoteFormat(IClient client, QuoteFormat quoteFormat)
		{
			quoteFormats[client] = quoteFormat;
		}

		private void OnOnNewClientConnected(IClient obj)
		{
			var handler = OnNewClientConnected;
			if (handler != null) handler(obj);
		}

		#endregion

		#region Events

		public event Action<IClient> OnNewClientConnected;

		#endregion
	}
}
