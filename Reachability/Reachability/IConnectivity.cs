using System;
using System.Collections.Generic;
namespace Reachability
{
	public interface IConnectivity
	{
		bool IsConnected();
		NetworkStatus ConnectedVia();
	}
}
