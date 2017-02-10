using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using System.IO;
using System.Net.Http.Headers;
using System.Diagnostics.Contracts;

namespace HTTPClient {
	/**
 * Client used to interact with the network context by HTTP requests.
 *
 * CustomHttpClient can make calls via GET or POST with the given params and retrieve the resulting string.
 */
	public class HttpClientManager {

		/** Set to true if you need to print the URLs calls and responses in the console. */
		private const bool HTTP_CONSOLE_ENABLED = true;
		/** Set the default request type. If you dont specify in the call, this will be used. */
		private static readonly HttpMethod DEFAULT_CONNECTION_METHOD = HttpMethod.GET;
		/** Define the time in milliseconds to cancel calls if server is not responding */
		private const int CONNECTION_TIMEOUT = 10000;
		/** Provide a list of possible responses to validate them; Set to null if you dont want to check */
		private static readonly String[] VALID_RESPONSES = { "true", "false", "0", "1", "2" };
		/** If your app will download JSONs, you can set a prefix to validate them. */
		public const String JSON_PREFIX = "JSONResponse";

		/** Apache HttpClient used for establish connection with server */
		private static System.Net.Http.HttpClient httpClient;

		/** Return the HttpClient with the default configuration */
		private static HttpClient GetHttpClient() {
			if (httpClient == null) {
				httpClient = new System.Net.Http.HttpClient();
				//httpClient.BaseAddress = new Uri(Constants.URLs.Build(""));
				httpClient.Timeout = TimeSpan.FromMilliseconds(CONNECTION_TIMEOUT);
			}
			return httpClient;
		}

		/** Execute a call to the given url with the default connection method.
		 *
		 * @param url to call as target of the request.
		 * @return the response text retrieved in the request.
		 */
		public static async Task<String> Execute(String url) {
			return await Execute(url, new List<KeyValuePair<String, String>>());
		}

		/** Execute a call to the given url with the given params with the default connection method.
	     *
	     * @param url to call as target of the request.
	     * @param params to send in the request.
	     * @return the response text retrieved in the request.
	     */
		public static async Task<String> Execute(String url, List<KeyValuePair<String, String>> parameters) {
			return await Execute(url, parameters, DEFAULT_CONNECTION_METHOD);
		}

		/** Execute a call to the given url with the given params with the specified connection method.
	     *
	     * @param url to call as target of the request.
	     * @param params to send in the request.
	     * @param method that will be used for the request.
	     * @return the response text retrieved in the request.
	     */
		public static async Task<String> Execute(String url, List<KeyValuePair<String, String>> parameters, HttpMethod method) {
			try {
				System.Net.Http.HttpClient client = GetHttpClient();
				HttpResponseMessage response = null;

				if (HTTP_CONSOLE_ENABLED) {
					Debug.WriteLine("HTTP: Start => " + url);
				}

				if (method == HttpMethod.GET) {
					url += "?" + ParamsToString(parameters);
					response = await client.GetAsync(url);
				} else if (method == HttpMethod.POST) {
					client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
					client.DefaultRequestHeaders.AcceptCharset.Add(new StringWithQualityHeaderValue("UTF-8"));
					client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("UTF-8"));

					HttpContent content = new FormUrlEncodedContent(parameters);
					response = await client.PostAsync(url, content);
				}

				if (HTTP_CONSOLE_ENABLED) {
					Debug.WriteLine("HTTP: End => " + url);
				}

				if (response.IsSuccessStatusCode) {
					if (HTTP_CONSOLE_ENABLED) {
						Debug.WriteLine("HTTP: Success!");
					}

					HttpContent content = response.Content;
					String result = await content.ReadAsStringAsync();
					if (result != null && result.Length > 0 && IsValidHttpResponse(result)) {
						return result;
					}
				}
				if (HTTP_CONSOLE_ENABLED) {
					Debug.WriteLine("HTTP: Failed!");
				}
			} catch (IOException e) {
				Debug.WriteLine("HTTP: Error => " + e.StackTrace);
			} catch (Exception e) {
				Debug.WriteLine("HTTP: Error => " + e.StackTrace);
			}
			return "false";
		}

		/** Method for encode a string with params as URL specification for "GET" connection method.
	     *
	     * @param params to encode in the string.
	     * @return the string containing the params encoded with UTF-8.
	     */
		public static String ParamsToString(List<KeyValuePair<String, String>> parameters) {
			StringBuilder sb = new StringBuilder();
			foreach (KeyValuePair<String, String> param in parameters) {
				sb.Append("");
				sb.Append(param.Key);
				sb.Append("=");
				if (param.Value != null) {
					byte[] bytes = Encoding.Convert(Encoding.Unicode, Encoding.UTF8, Encoding.Unicode.GetBytes(param.Value));
					String value = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
					sb.Append(value);
				} else {
					sb.Append("null");
				}
				sb.Append("&");
			}
			return sb.ToString();
		}

		/** Method to check if a response is valid to handle in the app or is just an error in server.
	     *
	     * @param response to check if is valid.
	     * @return true if pass all checks successfully, false if not.
	     */
		public static bool IsValidHttpResponse(String response) {
			if (VALID_RESPONSES != null) {
				String replacedResponse = response.Replace("\\n", "");    // Newline
				replacedResponse = replacedResponse.Replace("\\{", "");    // {
				replacedResponse = replacedResponse.Replace("\\}", "");    // }
				replacedResponse = replacedResponse.Replace(" ", "");    // White spaces
				replacedResponse = replacedResponse.Replace("\"", "");    // Double cuotes

				foreach (String validResponse in VALID_RESPONSES) {
					if (String.Compare(replacedResponse, validResponse, StringComparison.OrdinalIgnoreCase) == 0) return true;
				}

				if (replacedResponse.Length > JSON_PREFIX.Length && String.Compare(replacedResponse.Substring(0, JSON_PREFIX.Length), JSON_PREFIX, StringComparison.OrdinalIgnoreCase) == 0) return true;

				return false;
			} else {
				return true;
			}
		}

	}
}
