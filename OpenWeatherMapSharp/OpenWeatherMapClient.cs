using Newtonsoft.Json;
using OpenWeatherApiSharp.Exceptions;
using System;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace OpenWeatherApiSharp {
	public sealed class OpenWeatherMapClient : IDisposable {
		private const int MAX_RETRY_COUNT = 3;
		private const string BASE_URL = "https://api.openweathermap.org/data/2.5/weather";

		private readonly HttpClientHandler ClientHandler;
		private readonly HttpClient Client;
		private readonly SemaphoreSlim Sync = new SemaphoreSlim(1, 1);
		private readonly string AccessToken;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="_accessToken">The access token which grants access to the API.</param>
		/// <param name="_clientHandler">The http client handler. Default is null which will assign a default client handler.</param>
		public OpenWeatherMapClient(string _accessToken, HttpClientHandler _clientHandler = null) {
			AccessToken = _accessToken ?? throw new ArgumentNullException(nameof(_accessToken) + " cannot be null!");			
			ClientHandler = _clientHandler ?? new HttpClientHandler();
			Client = new HttpClient(ClientHandler, true);
		}

		private string GenerateRequestUrl(int locationCode, string countryCode) => $"{BASE_URL}?zip={locationCode},{countryCode}&appid={AccessToken}";

		/// <summary>
		/// sync method to get the weather response.
		/// </summary>
		/// <param name="_locPinCode">The pin code of the location.</param>
		/// <param name="_countryCode">The country code of the location country.</param>		
		/// <param name="maxRetryCount">The maximum number of tries the request has to retry when timeout errors occur.</param>
		/// <returns><see cref="WeatherResponse"/> result.</returns>
		/// <exception cref="ArgumentNullException"/>
		/// <exception cref="NetworkUnavailableException"/>
		public WeatherResponse GetWeather(int _locPinCode, string _countryCode, int maxRetryCount = MAX_RETRY_COUNT) => GetWeatherAsync(_locPinCode, _countryCode, default, maxRetryCount).Result;

		/// <summary>
		/// Async method to get the weather response.
		/// </summary>
		/// <param name="_locPinCode">The pin code of the location.</param>
		/// <param name="_countryCode">The country code of the location country.</param>
		/// <param name="token"><see cref="CancellationToken"/> of the task.</param>
		/// <param name="maxRetryCount">The maximum number of tries the request has to retry when timeout errors occur.</param>
		/// <returns><see cref="WeatherResponse"/> result.</returns>
		/// <exception cref="ArgumentNullException"/>
		/// <exception cref="NetworkUnavailableException"/>
		public async Task<WeatherResponse> GetWeatherAsync(int _locPinCode, string _countryCode, CancellationToken token = default, int maxRetryCount = MAX_RETRY_COUNT) {
			if(_locPinCode <= 0 || string.IsNullOrEmpty(_countryCode)) {
				throw new ArgumentNullException();
			}
			
			if (!await IsNetworkAvailable().ConfigureAwait(false)) {
				throw new NetworkUnavailableException();
			}

			await Sync.WaitAsync().ConfigureAwait(false);

			try {
				async Task<WeatherResponse> requestAction() {
					for (int i = 0; i < maxRetryCount; i++) {
						using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, GenerateRequestUrl(_locPinCode, _countryCode))) {
							using (HttpResponseMessage response = await Client.SendAsync(request, token).ConfigureAwait(false)) {
								if (response.StatusCode == HttpStatusCode.GatewayTimeout || response.StatusCode == HttpStatusCode.RequestTimeout) {
									continue;
								}

								if (response.StatusCode != HttpStatusCode.OK) {
									return default;
								}

								string responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

								if (string.IsNullOrEmpty(responseString)) {
									continue;
								}

								return JsonConvert.DeserializeObject<WeatherResponse>(responseString);
							}
						}
					}

					return default;
				}

				return await Task.Run(requestAction, token).ConfigureAwait(false);
			}
			finally {
				Sync.Release();
			}
		}

		private async Task<bool> IsNetworkAvailable() {
			using(Ping ping = new Ping()) {
				var reply = await ping.SendPingAsync(IPAddress.Parse("8.8.8.8")).ConfigureAwait(false);
				return reply.Status == IPStatus.Success;
			}
		}

		/// <summary>
		/// <inheritdoc />
		/// </summary>
		public void Dispose() {
			Client?.Dispose();
			Sync?.Dispose();
		}
	}
}
