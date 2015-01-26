using System.Threading;
using NUnit.Framework;

namespace Server.Tests
{
	[TestFixture]
    public class QuoteSourceTests
    {
		private QuotesSource source;

		[SetUp]
		public void Init()
		{
			source = new QuotesSource(1);
		}

		[Test]
		public void InitTest()
		{
			var wasActed = false;
			source.NewQuoteAvailable += quote =>
			{
				wasActed = true;
			};
			source.Start();
			Thread.Sleep(50);
			Assert.IsTrue(wasActed);
		}

		[Test]
		public void DeInitTest()
		{
			source.Start();
			source.Stop();
			var wasActed = false;
			source.NewQuoteAvailable += quote =>
			{
				wasActed = true;
			};
			Thread.Sleep(50);
			Assert.IsFalse(wasActed);
		}

		[Test]
		public void SequenceOfQuotesTest()
		{
			var counter = 0;
			source.NewQuoteAvailable += quote =>
			{
				counter++;
			};
			source.Start();
			Thread.Sleep(500);
			Assert.Greater(counter, 1);
		}

		[Test]
		public void DisposingTest()
		{
			var wasActed = false;
			using (var quotesSource = new QuotesSource(1))
			{
				quotesSource.NewQuoteAvailable += quote =>
				{
					wasActed = true;
				};
				quotesSource.Start();
			}
			wasActed = false;
			Thread.Sleep(50);
			Assert.IsFalse(wasActed);
		}
    }
}
