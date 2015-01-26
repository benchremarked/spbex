using System;
using System.IO;
using System.Threading;
using Server.Interfaces;
using Server.Models;

namespace Server.Network
{
	public class TcpClient : IClient
	{
		#region Fields

		private readonly System.Net.Sockets.TcpClient tcpClient;
		private readonly StreamWriter writeStream;
		private readonly StreamReader readStream;
		private bool isStarted = true;
		private bool isDisposed;

		#endregion

		#region Init/deinit

		public TcpClient(System.Net.Sockets.TcpClient tcpClient)
		{
			this.tcpClient = tcpClient;
			this.readStream = new StreamReader(tcpClient.GetStream());
			this.writeStream = new StreamWriter(tcpClient.GetStream());
			var readThread = new Thread(ReadThread);
			readThread.Start();
		}

		public void Dispose()
		{
			if (isDisposed)
				return;
			try
			{
				isDisposed = true;
				isStarted = false;
				readStream.Dispose();
				writeStream.Dispose();
				tcpClient.Close();
			}
			catch
			{
			}
		}

		#endregion

		#region Public methods

		public void SendData(string data)
		{
			try
			{
				writeStream.Write(data);
				writeStream.WriteLine();
				writeStream.Flush();
			}
			catch (Exception)
			{
				RaiseDisconnect();
			}
		}

		#endregion

		#region Private methods

		private void ReadThread()
		{
			while (isStarted)
			{
				try
				{
					var res = readStream.Read();
					switch (res)
					{
						case 'q':
							Dispose();
							RaiseDisconnect();
							break;
						case 'r':
							RaiseChangeQuoteFormat(this, QuoteFormat.Raw);
							break;
						case 'c':
							RaiseChangeQuoteFormat(this, QuoteFormat.Column);
							break;
						default:
							Console.WriteLine("Unknown command");
							break;
					}
				}
				catch (Exception ex)
				{
					Dispose();
					RaiseDisconnect();
				}
			}
		}

		private void RaiseDisconnect()
		{
			var handler = Disconnect;
			if (Disconnect != null) handler(this);
		}

		private void RaiseChangeQuoteFormat(IClient client, QuoteFormat format)
		{
			var handler = ChangeQuoteFormat;
			if (handler != null) handler(client, format);
		}

		#endregion

		#region Events

		public event Action<IClient> Disconnect;
		public event Action<IClient, QuoteFormat> ChangeQuoteFormat;

		#endregion
	}
}
