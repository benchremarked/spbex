using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Client
{
	class Program
	{
		private const string helpText = "press q for quit, r - row style, c - column style";

		private static bool isStarted = true;
		static void Main(string[] args)
		{
			try
			{
				var ip = "127.0.0.1";
				var port = 3333;

				var client = new TcpClient();
				client.Connect(IPAddress.Parse(ip), port);

				var thread = new Thread(ConsoleThread);
				thread.Start();
				readStream = new StreamReader(client.GetStream());
				writeStream = new StreamWriter(client.GetStream());

				while (isStarted)
				{
					var line = readStream.ReadLine();
					if (!string.IsNullOrEmpty(line))
					{
						ParseQuotes(line);
						PrintQuotes();
					}
					Thread.Sleep(300);
				}

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}

		private static void ConsoleThread()
		{
			while (true)
			{
				var key = Console.ReadKey();
				switch (key.KeyChar)
				{
					case 'q':
						writeStream.Write('q');
						writeStream.Flush();
						Console.WriteLine(" Disconnect");
						isStarted = false;
						return;
					case 'r':
						writeStream.Write('r');
						writeStream.Flush();
						Console.WriteLine(" Row style command");
						break;
					case 'c':
						writeStream.Write('c');
						writeStream.Flush();
						Console.WriteLine(" Column style command");
						break;
					default:
						Console.WriteLine(" Unknown command");
						break;
				}
			}
		}

		private static void PrintQuotes()
		{
			Console.Clear();
			foreach (var quote in currentMarketState.OrderBy(s => s.Key))
			{
				Console.Write("{0} {1} ", quote.Key, quote.Value.Price);

				var trendSymbol = "";
				switch (quote.Value.Trend)
				{
					case Trend.Increase:
						trendSymbol = "↑";
						Console.ForegroundColor = ConsoleColor.Green;
						break;
					case Trend.Decrease:
						trendSymbol = "↓";
						Console.ForegroundColor = ConsoleColor.Red;
						break;
				}
				Console.WriteLine(trendSymbol);
				Console.ResetColor();
			}
			Console.WriteLine();
			Console.WriteLine(helpText);
		}

		private static Dictionary<string, QuoteView> currentMarketState = new Dictionary<string, QuoteView>();
		private static StreamReader readStream;
		private static StreamWriter writeStream;

		static void ParseQuotes(string data)
		{
			var quotes = data.Split(new[] { ";", "/r/n" }, StringSplitOptions.RemoveEmptyEntries);
			foreach (var quote in quotes)
			{
				var items = quote.Split(new[] { '=' });
				if (items.Length != 2)
					continue;
				var symbol = items[0];
				var price = decimal.Parse(items[1]);

				var prevQuote = currentMarketState.ContainsKey(items[0]) ? currentMarketState[items[0]] : null;

				if (prevQuote == null)
					currentMarketState[items[0]] = new QuoteView
					{
						Price = price,
						Trend = Trend.None
					};
				else
				{
					currentMarketState[items[0]].Trend = currentMarketState[items[0]].Price < price ? Trend.Increase : Trend.Decrease;
					currentMarketState[items[0]].Price = price;
				}
			}
		}
	}

	public class QuoteView
	{
		public decimal Price { get; set; }
		public Trend Trend { get; set; }
	}

	public enum Trend
	{
		Increase,
		Decrease,
		None
	}
}
