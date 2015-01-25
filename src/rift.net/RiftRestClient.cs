using System;
using RestSharp;
using System.Collections.Generic;
using rift.net.rest;
using AutoMapper;

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
			var request = new RestRequest (url.Replace ("/chatservice", "/"), method);

			return request;
		}

		protected U ExecuteAndWrap<T,U>(RestRequest request) 
		{
			var response = Client.Execute(request);

			var content = SimpleJson.DeserializeObject<JsonResponse<T>> (response.Content);

			if ((content == null) || (content.status != "success"))
				throw new Exception ("An error occurred calling the service");

			return Mapper.Map<U> (content.data);
		}
	}
}

