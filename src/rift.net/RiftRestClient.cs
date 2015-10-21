using System;
using RestSharp;
using System.Collections.Generic;
using rift.net.rest;
using System.Threading.Tasks;
using System.Net;

namespace rift.net
{
	public class RiftRestClient
	{
		protected RestClient Client {
			get; 
			private set;
		}

		public RiftRestClient ()
		{
			Client = new RestClient (new Uri (Configuration.Url));
		}

		protected virtual RestRequest CreateRequest(string url, Method method = Method.GET)
		{
			var request = new RestRequest (url.Replace ("/chatservice", "/"), method);

			return request;
		}

		protected T Execute<T>(RestRequest request) 
		{
			if (Configuration.Timeout > 0)
				Client.Timeout = Configuration.Timeout;
			
			var response = Client.Execute(request);

			if ((response == null) || (response.ResponseStatus != ResponseStatus.Completed)) {
				throw new WebException (string.Format ("An error occurred making the call the service {0}", request.Resource), response.ErrorException);
			}

			var content = SimpleJson.DeserializeObject<JsonResponse<T>> (response.Content);

			if ((content == null) || (content.status != "success")) {
				throw new Exception  (string.Format ("An error occurred calling the service. {0}", response.Content));
			}

			return content.data;
		}

		protected async Task<T> ExecuteAsync<T>(RestRequest request)
		{
			var response = await Client.ExecuteTaskAsync (request);

			if ((response == null) || (response.ResponseStatus != ResponseStatus.Completed)) {
				throw new WebException (string.Format ("An error occurred making the call the service {0}", request.Resource), response.ErrorException);
			}

			var content = SimpleJson.DeserializeObject<JsonResponse<T>> (response.Content);

			if ((content == null) || (content.status != "success")) {
				throw new Exception (string.Format ("An error occurred calling the service. {0}", response.Content));
			}

			return content.data;
		}
	}
}

