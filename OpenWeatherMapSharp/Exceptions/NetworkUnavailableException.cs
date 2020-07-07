using System;
using System.Collections.Generic;
using System.Text;

namespace OpenWeatherMapSharp.Exceptions {
	public class NetworkUnavailableException : Exception {
		public NetworkUnavailableException() : base("Network connectivity is unavailable.") {

		}
	}
}
