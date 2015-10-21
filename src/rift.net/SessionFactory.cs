using System;
using RestSharp;
using System.Linq;
using System.Net;
using rift.net.Models;
using System.Diagnostics;

namespace rift.net
{
	public class SessionFactory
	{
		public Session Login(string username, string password)
		{
			var restClient = new RestClient ();
			restClient.BaseUrl = new Uri("https://auth.trionworlds.com/auth");

			var request = new RestRequest (Method.POST);
			request.AddHeader ("UserAgent", "trion/mobile");

			request.AddParameter ("username", username, ParameterType.GetOrPost);
			request.AddParameter ("password", password, ParameterType.GetOrPost);
			request.AddParameter ("channel", 1, ParameterType.GetOrPost);
			request.AddHeader ("Accept", "application/json");

			var response = restClient.Execute (request);

			if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized) {
				throw new AuthenticationException (username);
			} else if (response.ErrorException != null) {
				throw response.ErrorException;
			}

			return Login (response.Content);
		}

		public Session Login(string ticket) {
			
			var restClient = new RestClient ();
			restClient.BaseUrl = new Uri (Configuration.Url + "/loginByTicket?os=iOS&osVersion=5.100000&vendor=Apple");

			var request = new RestRequest (Method.POST);
			request.AddParameter ("ticket", ticket);

			var response = restClient.Execute(request);

			var cookie = response.Cookies.FirstOrDefault (x => x.Name == "SESSIONID");

			if (cookie == null)
				throw new InvalidSessionException ();

			var session = new Session (cookie.Value, ticket);

			return session;
		}
	}
}