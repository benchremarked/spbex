using System.Linq;
using NUnit.Framework;
using Server.Models;

namespace Server.Tests
{
	[TestFixture]
	public class QuoteStoreTests
	{
		[Test]
		public void OneQuoteTest()
		{
			var q = new Quote
			{
				Symbol = "EURUSD",
				Price = 1.0m
			};
			var store = new QuoteStore();
			store.PushQuote(q);

			var market = store.GetCurrentMarket();
			Assert.AreEqual(1, market.Count);
			Assert.AreEqual("EURUSD", market[0].Symbol);
			Assert.AreEqual(1.0, market[0].Price);
		}

		[Test]
		public void QuoteReplaceTest()
		{
			var q = new Quote
			{
				Symbol = "EURUSD",
				Price = 1.0m
			};
			var store = new QuoteStore();
			store.PushQuote(q);
			q.Price = 2.0m;
			store.PushQuote(q);

			var market = store.GetCurrentMarket();
			Assert.AreEqual(1, market.Count);
			Assert.AreEqual("EURUSD", market[0].Symbol);
			Assert.AreEqual(2.0, market[0].Price);
		}

		[Test]
		public void SomeQuotesTest()
		{
			var q1 = new Quote
			{
				Symbol = "EURUSD",
				Price = 1.0m
			};
			var q2 = new Quote
			{
				Symbol = "USDJPY",
				Price = 2.0m
			};
			var store = new QuoteStore();
			store.PushQuote(q1);
			store.PushQuote(q2);

			var market = store.GetCurrentMarket();
			Assert.AreEqual(2, market.Count);
			Assert.IsTrue(market.Any(quote => quote.Symbol == "EURUSD" && quote.Price == 1.0m));
			Assert.IsTrue(market.Any(quote => quote.Symbol == "USDJPY" && quote.Price == 2.0m));
		}

		[Test]
		public void GetChangesTest1()
		{
			var q = new Quote
			{
				Symbol = "EURUSD",
				Price = 1.0m
			};
			var store = new QuoteStore();
			store.PushQuote(q);
			q.Price = 2.0m;
			store.PushQuote(q);

			var market = store.GetChanges();
			Assert.AreEqual(1, market.Count);
			Assert.AreEqual("EURUSD", market[0].Symbol);
			Assert.AreEqual(2.0, market[0].Price);
		}

		[Test]
		public void GetChangesTest2()
		{
			var q1 = new Quote
			{
				Symbol = "EURUSD",
				Price = 1.0m
			};
			var q2 = new Quote
			{
				Symbol = "USDJPY",
				Price = 2.0m
			};
			var store = new QuoteStore();
			store.PushQuote(q1);
			store.PushQuote(q2);

			var market = store.GetChanges();
			Assert.AreEqual(2, market.Count);
			Assert.IsTrue(market.Any(quote => quote.Symbol == "EURUSD" && quote.Price == 1.0m));
			Assert.IsTrue(market.Any(quote => quote.Symbol == "USDJPY" && quote.Price == 2.0m));
		}

		[Test]
		public void GetChangesTest3()
		{
			var q1 = new Quote
			{
				Symbol = "EURUSD",
				Price = 1.0m
			};
			var q2 = new Quote
			{
				Symbol = "EURUSD",
				Price = 2.0m
			};
			var store = new QuoteStore();
			store.PushQuote(q1);
			store.PushQuote(q2);

			var market = store.GetChanges();
			Assert.AreEqual(1, market.Count);
			Assert.IsTrue(market.Any(quote => quote.Symbol == "EURUSD" && quote.Price == 2.0m));
		}
		[Test]
		public void ResetChangesTest1()
		{
			var q = new Quote
			{
				Symbol = "EURUSD",
				Price = 1.0m
			};
			var store = new QuoteStore();
			store.PushQuote(q);
			q.Price = 2.0m;
			store.PushQuote(q);
			store.ResetChanges();
			var market = store.GetChanges();
			Assert.AreEqual(0, market.Count);
		}

		[Test]
		public void ResetChangesTest2()
		{
			var q1 = new Quote
			{
				Symbol = "EURUSD",
				Price = 1.0m
			};
			var q2 = new Quote
			{
				Symbol = "USDJPY",
				Price = 2.0m
			};
			var store = new QuoteStore();
			store.PushQuote(q1);
			store.ResetChanges();
			store.PushQuote(q2);

			var market = store.GetChanges();
			Assert.AreEqual(1, market.Count);
			Assert.IsTrue(market.Any(quote => quote.Symbol == "USDJPY" && quote.Price == 2.0m));
		}

		
	}
}
