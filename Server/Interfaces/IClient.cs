using System;
using Server.Models;

namespace Server.Interfaces
{
	public interface IClient : IDisposable
	{
		void SendData(string data);

		event Action<IClient> Disconnect;
		event Action<IClient, QuoteFormat> ChangeQuoteFormat;

	}
}