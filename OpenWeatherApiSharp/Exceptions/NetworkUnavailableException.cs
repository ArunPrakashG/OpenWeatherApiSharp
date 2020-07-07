using System;
using System.Collections.Generic;
using System.Text;

namespace OpenWeatherApiSharp.Exceptions {
	public class NetworkUnavailableException : Exception {
		public NetworkUnavailableException() : base("Network connectivity is unavailable.") {

		}
	}
}
