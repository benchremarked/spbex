using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Server.Interfaces;
using Server.Models;

namespace Server.Tests
{
	[TestFixture]
	public class QuoteServerTests
	{
		private QuoteServer server;
		private Mock<IQuoteSource> quoteSource;
		private Mock<IQuoteTransportServer> transport;
		private Mock<IQuoteStore> quoteStore;

		[SetUp]
		public void Init()
		{
			quoteSource = new Mock<IQuoteSource>();
			transport = new Mock<IQuoteTransportServer>();
			quoteStore = new Mock<IQuoteStore>();

			server = new QuoteServer(quoteSource.Object, transport.Object, quoteStore.Object);	
		}

		[Test]
		public void SendingQuotesToNewClient()
		{
			server.Start();
			quoteStore.Setup(store => store.GetCurrentMarket()).Returns(() => new List<Quote>
			{
				new Quote
				{
					Symbol = "EURUSD",
					Price = 1.0m
				}
			});
			const string expected = "EURUSD=1.0;\r\n";
			var client = new Mock<IClient>();
			client.Setup(c => c.SendData(It.Is<string>(s => s == expected)));

			transport.Raise(transportServer => transportServer.OnNewClientConnected += null, client.Object);

			client.VerifyAll();
		}
	}
}
