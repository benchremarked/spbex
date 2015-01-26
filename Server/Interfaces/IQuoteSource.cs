using System;
using Server.Models;

namespace Server.Interfaces
{
	public interface IQuoteSource : IDisposable
	{
		event Action<Quote> NewQuoteAvailable;
		void Start();
	}
}