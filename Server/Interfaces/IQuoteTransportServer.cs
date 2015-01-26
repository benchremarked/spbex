using System;
using System.Collections.Generic;
using Server.Models;

namespace Server.Interfaces
{
	public interface IQuoteTransportServer : IDisposable
	{
		void SendToAll(IEnumerable<Quote> quotes);
		void Start();
		event Action<IClient> OnNewClientConnected;
	}
}
