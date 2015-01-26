using System.Collections.Generic;
using Server.Models;

namespace Server.Interfaces
{
	public interface IQuoteStore
	{
		void PushQuote(Quote quote);
		List<Quote> GetCurrentMarket();
		List<Quote> GetChanges();
		List<Quote> GetChangesAndReset();
		void ResetChanges();
	}
}
