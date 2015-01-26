using System;
using System.Collections.Generic;

namespace Server.Interfaces
{
	public interface ITransportServer : IDisposable
	{
		event Action<IClient> OnNewClientConnected;
		void Start();
		List<IClient> GetClients();
	}
}