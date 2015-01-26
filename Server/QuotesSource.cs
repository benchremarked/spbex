using System;
using System.Collections.Generic;
using System.Timers;
using Server.Interfaces;
using Server.Models;

namespace Server
{
	public sealed class QuotesSource : IQuoteSource
	{
		#region Fields

		private readonly int delay = 600; // delay in milliseconds

		private readonly Timer timer = new Timer();

		private readonly List<string> symbols = new List<string>
		{
			"RUB_EUR",
			"EUR_USD",
			"USD_CHF",
			"USD_AUD",
			"USD_GBP"
		};

		private bool isDisposed;

		private readonly Random rand = new Random();

		#endregion

		#region Init/Deinit

		public QuotesSource(int delay)
		{
			this.delay = delay;
			timer.Elapsed += OnTimer;
		}

		public void Dispose()
		{
			try
			{
				if (isDisposed) return;

				isDisposed = true;
				timer.Stop();
				timer.Elapsed -= OnTimer;
				timer.Dispose();
			}
			catch 
			{
			}
		}

		#endregion

		#region Public methods

		public void Start()
		{
			timer.Interval = delay;
			timer.Start();
		}

		public void Stop()
		{
			timer.Stop();
		}

		#endregion

		private void OnTimer(object sender, ElapsedEventArgs e)
		{
			var newQuote = GetNextQuote();
			OnNewQuoteAvailable(newQuote);
		}

		#region Private methods

		private Quote GetNextQuote()
		{
			return new Quote
			{
				Symbol = symbols[rand.Next(0, symbols.Count)],
				Price = (decimal)rand.NextDouble()
			};
		}

		private void OnNewQuoteAvailable(Quote quote)
		{
			var handler = NewQuoteAvailable;
			if (handler != null) handler(quote);
		}

		#endregion


		#region Events

		public event Action<Quote> NewQuoteAvailable;

		#endregion
	}
}
