using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace CMon.Services
{
	public interface IHttpClientFactory
	{
		IHttpClient CreateClient();
	}

	public interface IHttpClient : IDisposable
	{
		void AddHeader(string name, string value);

		Task<IHttpResponse> GetAsync(string requestUri);
	}

	public interface IHttpResponse
	{
		HttpStatusCode StatusCode { get; }

		Task<string> ReadContentAsync();
	}

	public class DefaultHttpClientFactory : IHttpClientFactory
	{
		public IHttpClient CreateClient()
		{
			return new HttpClientWrapper();
		}

		private class HttpClientWrapper : IHttpClient
		{
			private readonly HttpClient _client;

			public HttpClientWrapper()
			{
				_client = new HttpClient();
			}

			public void AddHeader(string name, string value)
			{
				_client.DefaultRequestHeaders.Add(name, value);
			}

			public async Task<IHttpResponse> GetAsync(string requestUri)
			{
				var response = await _client.GetAsync(requestUri);

				return new HttpResponseWrapper(response);
			}

			public void Dispose()
			{
				_client.Dispose();
			}
		}

		private class HttpResponseWrapper : IHttpResponse
		{
			private readonly HttpResponseMessage _response;

			public HttpResponseWrapper(HttpResponseMessage response)
			{
				_response = response;
			}

			public HttpStatusCode StatusCode => _response.StatusCode;

			public async Task<string> ReadContentAsync()
			{
				return await _response.Content.ReadAsStringAsync();
			}
		}
	}
}