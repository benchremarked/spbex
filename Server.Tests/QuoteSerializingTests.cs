using System.Collections.Generic;
using NUnit.Framework;
using Server.Models;

namespace Server.Tests
{
	[TestFixture]
	public class QuoteSerializingTests
	{
		[Test]
		public void QuoteSerializingTest()
		{
			var expect = "EURUSD=1.23";
			var q = new Quote
			{
				Symbol = "EURUSD",
				Price = 1.23m
			};

			var res = q.Serialize();
			Assert.AreEqual(expect, res);
		}

		[Test]
		public void QuoteListRowSerializingTest()
		{
			var expect = "EURUSD=1.23;USDJPY=2.34;";
			var quotes = new List<Quote>
			{
				new Quote
				{
					Symbol = "EURUSD",
					Price = 1.23m
				},
				new Quote
				{
					Symbol = "USDJPY",
					Price = 2.34m
				}
			};

			var res = quotes.Serialize();
			Assert.AreEqual(expect, res);
		}

		[Test]
		public void QuoteListColumnSerializingTest()
		{
			var expect = "EURUSD=1.23\r\n" +
			             "USDJPY=2.34\r\n";
			var quotes = new List<Quote>
			{
				new Quote
				{
					Symbol = "EURUSD",
					Price = 1.23m
				},
				new Quote
				{
					Symbol = "USDJPY",
					Price = 2.34m
				}
			};

			var res = quotes.Serialize(true);
			Assert.AreEqual(expect, res);
		}
	}
}
