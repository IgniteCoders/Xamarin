using System;
using Xamarin.Forms;
using Android.Net;
using Android.Content;
using Reachability.Droid;

[assembly: Dependency(typeof(Connectivity))]
namespace Reachability.Droid {
	public static class Reachability {
		public static void Init() {
			DependencyService.Register<Connectivity>();
		}
	}

	public class Connectivity : IConnectivity {
		private ConnectivityManager connectivityManager;
		private ReachabilityEventReceiver receiver;

		public Connectivity() {
			receiver = new ReachabilityEventReceiver();
			Android.App.Application.Context.RegisterReceiver(receiver, new IntentFilter(ConnectivityManager.ConnectivityAction));
		}

		private ConnectivityManager GetConnectivityManager() {
			if (connectivityManager == null || connectivityManager.Handle == IntPtr.Zero) {
				connectivityManager = (ConnectivityManager)(Android.App.Application.Context.GetSystemService(Context.ConnectivityService));
			}
			return connectivityManager;
		}

		public NetworkStatus ConnectedVia() {
			//When on API 21+ need to use getAllNetworks, else fall base to GetAllNetworkInfo
			//https://developer.android.com/reference/android/net/ConnectivityManager.html#getAllNetworks()
			if ((int)Android.OS.Build.VERSION.SdkInt >= 21) {
				foreach (Network network in GetConnectivityManager().GetAllNetworks()) {
					NetworkInfo info = GetConnectivityManager().GetNetworkInfo(network);

					if (info?.Type == null)
						return NetworkStatus.Other;


					return GetConnectionType(info.Type);
				}
			} else {
				foreach (NetworkInfo info in GetConnectivityManager().GetAllNetworkInfo()) {
					if (info?.Type == null)
						return NetworkStatus.Other;

					return GetConnectionType(info.Type);
				}
			}
			return NetworkStatus.NotReachable;
		}

		public bool IsConnected() {
			try {
				//When on API 21+ need to use getAllNetworks, else fall base to GetAllNetworkInfo
				//https://developer.android.com/reference/android/net/ConnectivityManager.html#getAllNetworks()
				if ((int)Android.OS.Build.VERSION.SdkInt >= 21) {
					foreach (Network network in GetConnectivityManager().GetAllNetworks()) {
						NetworkInfo info = GetConnectivityManager().GetNetworkInfo(network);

						if (info?.IsConnected ?? false)
							return true;
					}
				} else {
					foreach (NetworkInfo info in GetConnectivityManager().GetAllNetworkInfo()) {
						if (info?.IsConnected ?? false)
							return true;
					}
				}

				return false;
			} catch (Exception e) {
				Console.WriteLine("Unable to get connected state - do you have ACCESS_NETWORK_STATE permission? - error: {0}", e);
				return false;
			}
		}

		public static NetworkStatus GetConnectionType(ConnectivityType connectivityType) {
			switch (connectivityType) {
				case ConnectivityType.Wifi:
					return NetworkStatus.WiFiNetwork;
				case ConnectivityType.Mobile:
				case ConnectivityType.MobileDun:
				case ConnectivityType.MobileHipri:
					return NetworkStatus.DataNetwork;
				case ConnectivityType.Dummy:
				case ConnectivityType.Ethernet:
				case ConnectivityType.Wimax:
				case ConnectivityType.Bluetooth:
				default:
					return NetworkStatus.Other;
			}
		}

	}
}
