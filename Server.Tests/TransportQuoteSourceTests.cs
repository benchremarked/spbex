using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Server.Interfaces;
using Server.Models;

namespace Server.Tests
{
	[TestFixture]
	public class TransportQuoteSourceTests
	{
		private Mock<ITransportServer> transport;

		[SetUp]
		public void Init()
		{
			transport = new Mock<ITransportServer>();
		}

		[Test]
		public void NewClientNotificationTest()
		{
			var client = new Mock<IClient>();
			var wasActed = false;
			var server = new TransportQuoteServer(transport.Object);
			server.OnNewClientConnected += _ => wasActed = true;
			transport.Raise(transportServer => transportServer.OnNewClientConnected += null, client.Object);
			Assert.IsTrue(wasActed);
		}

		[Test]
		public void SendToAllTest()
		{
			var quotes = new List<Quote>
			{
				new Quote
				{
					Symbol = "EURUSD",
					Price = 1.0m
				}
			};
			var client = new Mock<IClient>();
			client.Setup(c => c.SendData("EURUSD=1.0;"));
			transport.Setup(transportServer => transportServer.GetClients()).Returns(() => new List<IClient> {client.Object});

			var server = new TransportQuoteServer(transport.Object);
			server.SendToAll(quotes);	
		
			client.VerifyAll();
		}
	}
}
