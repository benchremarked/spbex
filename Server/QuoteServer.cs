using System;
using System.Timers;
using Server.Interfaces;
using Server.Models;

namespace Server
{
	public class QuoteServer : IDisposable
	{
		#region Fields

		private readonly IQuoteSource quoteSource;
		private readonly IQuoteTransportServer server;
		private readonly IQuoteStore quoteStore;
		private readonly Timer timer = new Timer();

		#endregion

		public QuoteServer(IQuoteSource quoteSource, IQuoteTransportServer transportServer, IQuoteStore quoteStore)
		{
			this.quoteSource = quoteSource;
			this.server = transportServer;
			this.quoteStore = quoteStore;

			timer.Elapsed += TimerOnElapsed;
			timer.Interval = 2000;
			timer.Start();

			quoteSource.NewQuoteAvailable += QuoteSourceOnNewQuoteAvailable;
			server.OnNewClientConnected += ServerOnOnNewClientConnected;
		}

		public void Start()
		{
			server.Start();
			quoteSource.Start();
		}

		private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
		{
			var changes = quoteStore.GetChangesAndReset();
			server.SendToAll(changes);
		}

		private void ServerOnOnNewClientConnected(IClient client)
		{
			var market = quoteStore.GetCurrentMarket();
			var data = market.Serialize() + Environment.NewLine;
			client.SendData(data);
		}

		private void QuoteSourceOnNewQuoteAvailable(Quote quote)
		{
			quoteStore.PushQuote(quote);
		}

		public void Dispose()
		{
			try
			{
				quoteSource.NewQuoteAvailable -= QuoteSourceOnNewQuoteAvailable;
				server.OnNewClientConnected -= ServerOnOnNewClientConnected;

				quoteSource.Dispose();
				server.Dispose();
				timer.Dispose();
			}
			catch 
			{
			}
		}
	}
}
