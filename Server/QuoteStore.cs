using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Server.Interfaces;
using Server.Models;

namespace Server
{
	public class QuoteStore : IQuoteStore
	{
		private readonly ConcurrentDictionary<string, decimal> market = new ConcurrentDictionary<string, decimal>();
		private readonly ConcurrentDictionary<string, decimal> changes = new ConcurrentDictionary<string, decimal>();
		private readonly object syncChanges = new object();

		public void PushQuote(Quote quote)
		{
			market[quote.Symbol] = quote.Price;

			// lock for atomic get and reset
			lock (syncChanges) // ReaderWriterLock for improve perfomance may be used
			{
				changes[quote.Symbol] = quote.Price;
			}
		}

		public List<Quote> GetCurrentMarket()
		{
			return market.Select(pair => new Quote { Symbol = pair.Key, Price = pair.Value }).ToList();
		}

		public List<Quote> GetChanges()
		{
			return changes.Select(pair => new Quote {Price = pair.Value, Symbol = pair.Key}).ToList();
		}

		public List<Quote> GetChangesAndReset()
		{
			// lock for atomic get and reset
			lock (syncChanges)
			{
				var c = changes.Select(pair => new Quote {Price = pair.Value, Symbol = pair.Key}).ToList();
				changes.Clear();
				return c;
			}
		}

		public void ResetChanges()
		{
			// Not need lock
			changes.Clear();
		}
	}
}