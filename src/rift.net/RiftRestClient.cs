using System;
using RestSharp;

namespace rift.net
{
	public class RiftRestClient
	{
		private const string url = "http://chat-us.riftgame.com:8080/chatservice";

		protected RestClient Client {
			get; 
			private set;
		}

		public RiftRestClient ()
		{
			Client = new RestClient (new Uri (url));
		}

		protected virtual RestRequest CreateRequest(string url, Method method = Method.POST)
		{
			var request = new RestRequest (url, method);

			return request;
		}
	}
}

