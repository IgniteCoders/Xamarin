using System;
using SystemConfiguration;
using System.Collections.Generic;
using Xamarin.Forms;
using Foundation;
using CoreFoundation;
using Reachability;
using Reachability.iOS;
using System.Threading.Tasks;

[assembly: Dependency(typeof(Connectivity))]
namespace Reachability.iOS {
	public static class Reachability {
		public static void Init() {
			DependencyService.Register<Connectivity>();
		}
	}

	public class Connectivity : IConnectivity {
		private NetworkReachability reachability;

		public Connectivity() {
			reachability = new NetworkReachability("www.google.com");
			reachability.SetNotification(OnChange);
			reachability.Schedule(CFRunLoop.Current, CFRunLoop.ModeDefault);
		}

		private bool IsNetworkAvailable(out NetworkReachabilityFlags flags) {
			return reachability.TryGetFlags(out flags) && IsReachableWithoutRequiringConnection(flags);
		}

		private NetworkStatus GetNetworkStatus() {
			NetworkReachabilityFlags flags;
			bool defaultNetworkAvailable = IsNetworkAvailable(out flags);

			if (defaultNetworkAvailable && ((flags & NetworkReachabilityFlags.IsDirect) != 0))
				return NetworkStatus.NotReachable;

			if ((flags & NetworkReachabilityFlags.IsWWAN) != 0)
				return NetworkStatus.DataNetwork;

			if (flags == 0)
				return NetworkStatus.NotReachable;

			return NetworkStatus.WiFiNetwork;
		}

		private bool IsReachableWithoutRequiringConnection(NetworkReachabilityFlags flags) {
			// Is it reachable with the current network configuration?
			bool isReachable = (flags & NetworkReachabilityFlags.Reachable) != 0;

			// Do we need a connection to reach it?
			bool noConnectionRequired = (flags & NetworkReachabilityFlags.ConnectionRequired) == 0
				|| (flags & NetworkReachabilityFlags.IsWWAN) != 0;

			return isReachable && noConnectionRequired;
		}

		private void OnChange(NetworkReachabilityFlags flags) {
			Task.Factory.StartNew(async() => {
				await NotifyChanges(flags);
			});
		}

		async Task NotifyChanges(NetworkReachabilityFlags flags) {
			await Task.Delay(100); // Necesita un tiempo... si no no llega a cambiar el estado
			NSNotificationCenter.DefaultCenter.PostNotificationName(NetworkManager.EVENT_NAME, null, null);
		}

		public bool IsConnected() {
			NetworkReachabilityFlags flags;
			return IsNetworkAvailable(out flags);
		}

		public NetworkStatus ConnectedVia() {
			return GetNetworkStatus();
		}
	}
}
