using System;
using Xamarin.Forms;
namespace Reachability
{
	public class NetworkManager
	{
		public static String EVENT_NAME = "network_connectivity_changed";
		private static IConnectivity connectivity = DependencyService.Get<IConnectivity>();

		public NetworkManager()
		{

		}

		public static bool IsConnected() { return connectivity.IsConnected(); }
		public static NetworkStatus ConnectedVia() { return connectivity.ConnectedVia(); }
	}
}
