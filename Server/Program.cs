using System;
using Server.Network;

namespace Server
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Initializing server...");
			Console.Write("Initializing quote source...");

			using (var quoteSource = new QuotesSource(600))
			{
				Console.WriteLine("done");
				Console.Write("Initializing quote store...");

				var quoteStore = new QuoteStore();
				Console.WriteLine("done");
				Console.Write("Initializing TcpServer...");
				using (var tcpServer = new TcpServer())
				{
					using (var transportServer = new TransportQuoteServer(tcpServer))
					{
						Console.WriteLine("done");
						Console.Write("Initializing QuoteServer...");

						using (var server = new QuoteServer(quoteSource, transportServer, quoteStore))
						{
							Console.WriteLine("done");

							server.Start();
							Console.WriteLine("Server started.");
							Console.WriteLine("Press any key to close");
							Console.ReadKey();
						}
					}
				}
			}
		}
	}
}
